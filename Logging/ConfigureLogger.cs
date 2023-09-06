using log4net;
using log4net.Config;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.Reflection;

namespace Logging
{
    public static class ConfigureLogger
    {
        public static IServiceCollection AddLog4Net(this IServiceCollection services, string fileName)
        {
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo(fileName));

            services.AddTransient<ILogger.ILogger, Logger.Logger>();
            services.AddTransient<ILogger.ILoggerFactory, Logger.LoggerFactory>();
            return services;
        }
    }
}
