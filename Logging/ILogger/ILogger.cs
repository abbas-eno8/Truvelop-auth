using System;

namespace Logging.ILogger
{
    public interface ILogger
    {
        void Error(object message);
        void Error(object message, Exception exception);

        void Info(object message);
        void Info(object message, Exception exception);

        void Warn(object message);
        void Warn(object message, Exception exception);

        void SetLoggerProperty(string propertyName, string propertyValue);
    }
}
