using AuthoritySTS.Data;
using AuthoritySTS.Models;
using AuthoritySTS.Services;
using AuthoritySTS.Services.BLL;
using AuthoritySTS.Services.Factory;
using AuthoritySTS.Services.Factory.Interface;
using AuthoritySTS.Services.Interface;
using AuthoritySTS.Services.Shared;
using IdentityServer4;
using Logging;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IO;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.DataProtection;
using System.Net;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.IdentityModel.Logging;

namespace AuthoritySTS
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        private AzureAdAuthentications _azureAdAuthentications;

        private ILogger _logger;

        public Startup(IConfiguration configuration, IHostingEnvironment env, ILogger<Startup> logger)
        {
            var builder = new ConfigurationBuilder()
             .SetBasePath(Directory.GetCurrentDirectory())
             .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
             .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);
            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
            _logger = logger;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();
            services.AddAuthentication().AddGoogle(Messages.Google, options =>
            {
                options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                options.ClientId = Configuration.GetSection("appSettings").GetSection("GoogleClientId").Value;
                options.ClientSecret = Configuration.GetSection("appSettings").GetSection("GoogleClientSecret").Value;
                options.UserInformationEndpoint = Messages.UserInformationEndpoint;
                options.ClaimActions.Clear();
                options.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "id");
                options.ClaimActions.MapJsonKey(ClaimTypes.Name, "name");
                options.ClaimActions.MapJsonKey(ClaimTypes.GivenName, "given_name");
                options.ClaimActions.MapJsonKey(ClaimTypes.Surname, "family_name");
                options.ClaimActions.MapJsonKey("urn:google:profile", "link");
                options.ClaimActions.MapJsonKey(ClaimTypes.Email, "email");
            });
            //Azure AD configuration

            services.AddTransient<AzureAdAuthentications>();

            var optionsBuilder = new DbContextOptionsBuilder<TriggerDbContext>();
            optionsBuilder.UseSqlServer(Configuration.GetConnectionString(Messages.AuthorityConnectionString));
            TriggerDbContext dbContext = new TriggerDbContext(optionsBuilder.Options);
            _azureAdAuthentications = new AzureAdAuthentications(dbContext, _logger);
            //services.AddAuthentication();
            _azureAdAuthentications.AzureAdConfiguration(services);

            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString(Messages.AuthorityConnectionString)));

            services.AddDbContext<TriggerDbContext>(options =>
               options.UseSqlServer(Configuration.GetConnectionString(Messages.AuthorityConnectionString)));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddEntityFrameworkStores<TriggerDbContext>()
                .AddDefaultTokenProviders()
            .AddTokenProvider<ConfirmEmailDataProtectorTokenProvider<ApplicationUser>>(Messages.EmailConfirmationTokenProviderName);

            var cert = new System.Security.Cryptography.X509Certificates.X509Certificate2(System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "identityserver.pfx"), "password");
            // services.AddIdentityServer()
            services.AddIdentityServer(options =>
                 {
                     options.Endpoints.EnableDeviceAuthorizationEndpoint = false;
                 })
              .AddSigningCredential(cert)
             //.AddDeveloperSigningCredential()
             .AddInMemoryApiResources(Config.GetApiResources())
             .AddInMemoryClients(Config.GetClients(Configuration))//Configuration
             .AddInMemoryIdentityResources(Config.GetIdentityResources())             
              //.AddInMemoryPersistedGrants()
              // this adds the operational data from DB (codes, tokens, consents)

              .AddOperationalStore(options =>
              {
                  options.ConfigureDbContext = builder =>
                      builder.UseSqlServer(Configuration.GetConnectionString(Messages.AuthorityConnectionString),
                          sql => sql.MigrationsAssembly(migrationsAssembly));
              })
             //this is used for user management   
             .AddAspNetIdentity<ApplicationUser>()
            .AddProfileService<ProfileService>();
            services.AddTransient<IdentityServer4.Validation.IResourceOwnerPasswordValidator, ResourceOwnerPasswordValidator>();

            services.AddAuthentication().AddJwtBearer(
                jwt =>
                {
                    jwt.Authority = Configuration.GetSection("appSettings").GetSection("AuthUrl").Value;
                    jwt.RequireHttpsMetadata = Convert.ToBoolean(Configuration.GetSection("appSettings").GetSection("HttpsMetadata").Value);
                    jwt.Audience = "TriggerClientApi";
                }
                );
            services.Configure<IdentityOptions>(options =>
            {
                // Lockout settings
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromHours(6);
                options.Lockout.MaxFailedAccessAttempts = 3;
                options.Lockout.AllowedForNewUsers = false;
                options.Tokens.EmailConfirmationTokenProvider = Messages.EmailConfirmationTokenProviderName;
                options.Tokens.PasswordResetTokenProvider = Messages.EmailConfirmationTokenProviderName;
            });

            services.Configure<ConfirmEmailDataProtectionTokenProviderOptions>(options =>
            {
                options.TokenLifespan = TimeSpan.FromMinutes(10);
            });

            services.AddMvc();
            services.AddTransient<IBuildLoginView, BuildLoginView>();
            services.AddTransient<IEmailSender, EmailSender>();
            services.AddTransient<IServiceFactory, ServiceFactory>();
            services.AddTransient<IMailMessageFactory, MailMessageFactory>();
            services.AddTransient<IUserDetails, UserDetails>();
            services.AddSingleton<OpenIdConnectPostConfigureOptions>();
            services.AddLog4Net("log4net.config");
            services.AddMvc(options => options.EnableEndpointRouting = false);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {

            app.UseExceptionHandler(errorApp =>
            {
                errorApp.Run(async context =>
                {
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    context.Response.ContentType = "text/html";
                    var ex = context.Features.Get<IExceptionHandlerFeature>();
                    if (ex != null)
                    {
                        var errorPage = "/Errors/Error"; // customize this to your error page path
                        context.Response.Redirect(errorPage);
                    }
                });
            });

            app.UseStaticFiles();
            app.UseCors(options =>
                options.WithOrigins("*").AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin()
            );
            app.UseIdentityServer();
            app.UseAuthentication();
            app.UseRequestLocalization();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=account}/{action=login}/{id?}");
            });
        }
    }

}
