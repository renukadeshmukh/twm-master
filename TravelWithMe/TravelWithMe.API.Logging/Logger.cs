using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TravelWithMe.API.Core.Infra;
using System.Threading.Tasks;
using TravelWithMe.Data.MySql;
using TravelWithMe.Logging.Helper;

namespace TravelWithMe.API.Logging
{
    public class Logger
    {
        public static void LogException(Exception exception, string source, string method, Severity severity)
        {
            var log = new ExceptionLog(exception, source, method, severity);
            log.SessionId = ApplicationContext.GetSessionId();
            var dataProvider = new LoggingDataProvider();
            if (LoggingConfigurations.IsLogAsyncEnabled)
            {
                Task.Factory.StartNew(() => new LoggingDataProvider().SaveException(log));
            }
            else
            {
                new LoggingDataProvider().SaveException(log);
            }
        }

        public static void LogMessage(Log log)
        {
            if (LoggingConfigurations.IsLoggingEnabled)
            {
                log.SessionId = ApplicationContext.GetSessionId();
                var dataProvider = new LoggingDataProvider();

                if (LoggingConfigurations.IsLogAsyncEnabled)
                {
                    Task.Factory.StartNew(() => new LoggingDataProvider().SaveLog(log));
                }
                else
                {
                    new LoggingDataProvider().SaveLog(log);
                }
            }
        }
    }
}
