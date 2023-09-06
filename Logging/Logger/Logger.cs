using Logging.ILogger;
using System;

namespace Logging.Logger
{
    public class Logger : ILogger.ILogger
    {
        private readonly log4net.ILog _log;//= log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly ILoggerFactory _loggerFactory;

        public Logger(ILoggerFactory loggerFactory)
        {
            this._loggerFactory = loggerFactory;
            _log = loggerFactory.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        }

        public void Error(object message)
        {
            SetLoggerConfig();
            _log.Error(message);
        }
        public void Error(object message, Exception exception)
        {
            SetLoggerConfig();
            _log.Error(message, exception);
        }

        public void Info(object message)
        {
            SetLoggerConfig();
            _log.Info(message);
        }

        public void Info(object message, Exception exception)
        {
            SetLoggerConfig();
            _log.Info(message, exception);
        }
        public void Warn(object message)
        {
            SetLoggerConfig();
            _log.Warn(message);
        }

        public void Warn(object message, Exception exception)
        {
            SetLoggerConfig();
            _log.Warn(message, exception);
        }

        private void SetLoggerConfig()
        {
            var stackTrace = _loggerFactory.GetStackTrace();
            SetLoggerProperty("Method", stackTrace.GetFrame(5).GetMethod().Name);
            if(stackTrace.GetFrame(5).GetMethod().DeclaringType!= null)
                SetLoggerProperty("ClassName", stackTrace.GetFrame(5).GetMethod().DeclaringType.FullName);
        }

        public void SetLoggerProperty(string propertyName, string propertyValue)
        {
            log4net.LogicalThreadContext.Properties[propertyName] = propertyValue;
        }
    }
}
