using System;
using System.Configuration;

namespace TravelWithMe.API.MobileMgmt
{
    public static class Configuration
    {
        public static bool IsMobileMock
        {
            get
            {
                return
                    !string.Equals(ConfigurationManager.AppSettings["IsMobileMock"], "N",
                                   StringComparison.OrdinalIgnoreCase);
            }
        }

        public static string MobileVerificationBaseUrl
        {
            get { return ConfigurationManager.AppSettings["MobileVerificationBaseUrl"]; }
        }

        public static string MobileVerificationFormat
        {
            get { return ConfigurationManager.AppSettings["MobileVerificationFormat"]; }
        }
    }
}