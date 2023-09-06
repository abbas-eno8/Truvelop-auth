using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MvcClient
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
            services.AddMvc();

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = "MyCookies";
                options.DefaultChallengeScheme = "oidc";
            })
            .AddCookie("MyCookies")

            .AddOpenIdConnect("oidc", options =>
             {
                 options.SignInScheme = "MyCookies";
                 options.Authority = "http://localhost:5000/";
                 options.RequireHttpsMetadata = false;
                 options.SaveTokens = true;
                 //Additional For hybrid
                 options.ClientId = "hybrid";
                 options.ClientSecret = "secret";
                 options.ResponseType = "code id_token";
                 options.GetClaimsFromUserInfoEndpoint = true;

                 options.Scope.Add("TriggerApi");
                 options.Scope.Add("offline_access");

                 ////SSO Override code
                 //options.Events.OnRedirectToIdentityProvider = context =>
                 //{
                 //    context.ProtocolMessage.Prompt = "login";
                 //    return Task.CompletedTask;
                 //};
             });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                //app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseAuthentication();

            app.UseStaticFiles();
            app.UseRouting();

            // Define your routes using UseEndpoints
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                // Add more routes here...
            });
            //app.UseEndpoints(endpoits => {
            //    endpoits.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
            //});

            //app.UseMvc(routes =>
            //{
            //    routes.MapRoute(
            //        name: "default",
            //        template: "{controller=Home}/{action=Index}/{id?}");
            //});
        }
    }
}
