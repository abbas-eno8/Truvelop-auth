using Logging.ILogger;
using System;
using System.Diagnostics;

namespace Logging.Logger
{
    internal class LoggerFactory : ILoggerFactory
    {
        public log4net.ILog GetLogger(Type type)
        {
            return log4net.LogManager.GetLogger(type);
        }

        public StackTrace GetStackTrace()
        {
            return new StackTrace();
        }
    }
}
