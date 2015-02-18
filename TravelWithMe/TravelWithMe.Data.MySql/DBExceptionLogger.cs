using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TravelWithMe.Logging.Helper;
using TravelWithMe.API.Core.Infra;

namespace TravelWithMe.Data.MySql.Entities
{
    public class DBExceptionLogger
    {
        public static void LogException(Exception exception, string source, string method, Severity severity)
        {
            var log = new ExceptionLog(exception, source, method, severity);
            log.SessionId = ApplicationContext.GetSessionId();
            var dataProvider = new LoggingDataProvider();
            if (DbConfiguration.IsLogAsyncEnabled)
            {
                Task.Factory.StartNew(() => new LoggingDataProvider().SaveException(log));
            }
            else
            {
                new LoggingDataProvider().SaveException(log);
            }
        }
    }
}
