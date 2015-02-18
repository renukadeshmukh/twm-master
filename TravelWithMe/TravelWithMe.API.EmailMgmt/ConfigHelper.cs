using System;
using System.Configuration;

namespace TravelWithMe.API.EmailMgmt
{
    public static class ConfigHelper
    {
        public static string ImapEmailHost
        {
            get { return ConfigurationManager.AppSettings["ImapHost"]; }
        }

        public static int ImapPort
        {
            get { return int.Parse(ConfigurationManager.AppSettings["ImapPort"]); }
        }

        public static bool IsImapSslEnabled
        {
            get
            {
                return string.Equals(ConfigurationManager.AppSettings["IsImapSslEnabled"], "Y",
                                     StringComparison.OrdinalIgnoreCase);
            }
        }

        public static string CustomerCareUsername
        {
            get { return ConfigurationManager.AppSettings["CustomerCareUsername"]; }
        }

        public static string CustomerCareEmail
        {
            get { return ConfigurationManager.AppSettings["CustomerCareEmail"]; }
        }

        public static string CustomerCarePassword
        {
            get { return ConfigurationManager.AppSettings["CustomerCarePassword"]; }
        }

        public static string SmtpEmailHost
        {
            get { return ConfigurationManager.AppSettings["SmtpHost"]; }
        }

        public static int SmtpPort
        {
            get { return int.Parse(ConfigurationManager.AppSettings["SmtpPort"]); }
        }

        public static bool IsSmtpSslEnabled
        {
            get
            {
                return string.Equals(ConfigurationManager.AppSettings["IsSmtpSslEnabled"], "Y",
                                     StringComparison.OrdinalIgnoreCase);
            }
        }

        public static string RedirectUrl
        {
            get { return ConfigurationManager.AppSettings["RedirectUrl"]; }
        }

        public static string EmailVerificationBaseUrl
        {
            get { return ConfigurationManager.AppSettings["EmailVerificationBaseUrl"]; }
        }

        public static string EmailTemplatesPath
        {
            get { return ConfigurationManager.AppSettings["EmailTemplatesPath"]; }
        }

        public static string BaseSiteUrl
        {
            get
            {
                string config = ConfigurationManager.AppSettings["BaseSiteUrl"];
                return string.IsNullOrEmpty(config) ? "https://www.BusSwitch.com" : config;
            }
        }
    }
}