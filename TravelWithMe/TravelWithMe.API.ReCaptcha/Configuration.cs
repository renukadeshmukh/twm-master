using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace TravelWithMe.API.ReCaptcha
{
    public static class Configuration
    {
        public static string ReCaptchaUrl
        {
            get
            {
                return string.IsNullOrEmpty(ConfigurationManager.AppSettings["ReCaptcha.Url"])
                           ? "http://www.google.com/recaptcha/api/verify"
                           : ConfigurationManager.AppSettings["ReCaptcha.Url"];
            }
        }

        public static string ReCaptchaPrivateKey
        {
            get
            {
                return string.IsNullOrEmpty(ConfigurationManager.AppSettings["ReCaptcha.PrivateKey"])
                           ? "6Ldwr9USAAAAAGEJTRBD--5p6OGP5F56W3JaQkGK"
                           : ConfigurationManager.AppSettings["ReCaptcha.PrivateKey"];
            }
        }

        public static string ReCaptchaPublicKey
        {
            get
            {
                return string.IsNullOrEmpty(ConfigurationManager.AppSettings["ReCaptcha.PublicKey"])
                           ? "6Ldwr9USAAAAAFDfmEXwDbjDOKc5Iy9qvbF4RoY2"
                           : ConfigurationManager.AppSettings["ReCaptcha.PrivateKey"];
            }
        }
    }
}
