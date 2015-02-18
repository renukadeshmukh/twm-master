using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace TravelWithMe.Data.MySql
{
    public static class DbConfiguration
    {
        public static string DatabaseRead
        {
            get { return ConfigurationManager.ConnectionStrings["MySqlDB.Read"].ConnectionString; }
        }

        public static string DatabaseWrite
        {
            get { return ConfigurationManager.ConnectionStrings["MySqlDB.Write"].ConnectionString; }
        }

        public static string BookingDB
        {
            get { return ConfigurationManager.ConnectionStrings["BookingDB"].ConnectionString; }
        }

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

        public static string LogDB
        {
            get { return ConfigurationManager.ConnectionStrings["Logging"].ConnectionString; }
        }
    }
}
