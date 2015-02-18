using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace TravelWithMe.API.Core.Factories
{
    public static class Configuration
    {
        public static bool IsMock
        {
            get { return string.Equals(ConfigurationManager.AppSettings["IsMock"], "Y", StringComparison.OrdinalIgnoreCase); }
        }

        public static bool IsMockProvider(string provider)
        {
            string config = ConfigurationManager.AppSettings["IsMockProvider"];
            if (string.IsNullOrEmpty(config) || string.IsNullOrEmpty(provider)) return false;
            List<string> configs = config.Split('|').ToList();
            foreach (string c in configs)
            {
                if (c.Contains(provider))
                {
                    string[] keyval = c.Split(':');
                    return keyval.Length < 2 || string.Equals("N", keyval[1], StringComparison.OrdinalIgnoreCase) ? false : true;
                }
            }
            return false;
        }
    }
}
