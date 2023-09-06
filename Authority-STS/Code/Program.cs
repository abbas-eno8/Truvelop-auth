using Microsoft.AspNetCore;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AuthoritySTS
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }
        
        
        public static IWebHost BuildWebHost(string[] args) =>
          WebHost.CreateDefaultBuilder(args)
              .UseStartup<Startup>()
           //.UseKestrel(o => { o.Limits.MaxRequestBodySize = null; })
              .ConfigureLogging((hostingContext, logging) =>
              {
                  logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                  logging.AddLog4Net();
              })
              .Build();
    }
}
