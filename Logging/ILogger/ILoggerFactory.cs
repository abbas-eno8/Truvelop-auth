using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Logging.ILogger
{
    public interface ILoggerFactory
    {
        log4net.ILog GetLogger(Type type);
        StackTrace GetStackTrace();
    }
}
