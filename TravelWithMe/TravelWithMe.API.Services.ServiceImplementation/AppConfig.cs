using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace TravelWithMe.API.Services.ServiceImplementation
{
    public class AppConfig
    {
        public static string CookieName
        {
            get
            {
                return string.IsNullOrEmpty(ConfigurationManager.AppSettings["CookieName"])
                           ? "Session"
                           : ConfigurationManager.AppSettings["CookieName"];
            }
        }

        public static string BaseSiteUrl
        {
            get
            {
                return string.IsNullOrEmpty(ConfigurationManager.AppSettings["BaseSiteUrl"])
                           ? "http://localhost:8971"
                           : ConfigurationManager.AppSettings["BaseSiteUrl"];
            }
        }
    }
}
