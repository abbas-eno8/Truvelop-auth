using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Authority.Data;
using Authority.Models;
using Authority.Services;
using System.Reflection;

namespace Authority
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddIdentityServer()
             .AddDeveloperSigningCredential()
             .AddInMemoryApiResources(Config.GetApiResources())
             .AddInMemoryClients(Config.GetClients())
             .AddInMemoryIdentityResources(Config.GetIdentityResources())
             .AddInMemoryPersistedGrants()

             // this adds the config data from DB (clients, resources)
             //.AddConfigurationStore(options =>
             //{
             //    options.ConfigureDbContext = builder =>
             //        builder.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"),
             //            sql => sql.MigrationsAssembly(migrationsAssembly));
             //})
             //.AddTestUsers(Config.GetUsers());
             //// this adds the operational data from DB (codes, tokens, consents)
             //.AddOperationalStore(options =>
             //{
             //    options.ConfigureDbContext = builder =>
             //        builder.UseSqlServer(configuration.GetConnectionString("1AuthorityConnection"),
             //            sql => sql.MigrationsAssembly(migrationsAssembly));
             //    // this enables automatic token cleanup. this is optional.
             //    options.EnableTokenCleanup = true;
             //    options.TokenCleanupInterval = 30;
             //})
             //this is used for user management   
             .AddAspNetIdentity<ApplicationUser>();
            //.AddProfileService<ProfileService>();
            // Add application services.
            services.AddTransient<IEmailSender, EmailSender>();

            services.AddMvc();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseIdentityServer();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
