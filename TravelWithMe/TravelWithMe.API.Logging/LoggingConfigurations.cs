using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace TravelWithMe.API.Logging
{
    public static class LoggingConfigurations
    {
        public static bool IsLogAsyncEnabled
        {
            get
            {
                string config = ConfigurationManager.AppSettings["IsLogAsyncEnabled"];
                if (!string.IsNullOrEmpty(config) &&
                    string.Equals(ConfigurationManager.AppSettings["IsLogAsyncEnabled"], "N",
                                  StringComparison.OrdinalIgnoreCase))
                    return false;
                return true;
            }
        }

        public static bool IsLoggingEnabled
        {
            get
            {
                string config = ConfigurationManager.AppSettings["IsLoggingEnabled"];
                if (!string.IsNullOrEmpty(config) &&
                    string.Equals(ConfigurationManager.AppSettings["IsLoggingEnabled"], "N",
                                  StringComparison.OrdinalIgnoreCase))
                    return false;
                return true;
            }
        }
    }
}
