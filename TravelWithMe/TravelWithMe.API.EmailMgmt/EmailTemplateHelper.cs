using System.IO;

namespace TravelWithMe.API.EmailMgmt
{
    public sealed class EmailTemplateHelper
    {
        private static string _registrationTemplate;

        private static string _forgotPasswordTemplate;

        private static string _emailVerificationTemplate;

        private static string _requestDealsTemplate;

        private static string _requestSubmittedTemplate;

        private static string _travellerTemplate;

        public static string RegistrationTemplate
        {
            get
            {
                if (string.IsNullOrEmpty(_registrationTemplate))
                {
                    _registrationTemplate = File.ReadAllText(ConfigHelper.EmailTemplatesPath + "Register.htm");
                }
                return _registrationTemplate;
            }
        }

        public static string ForgotPasswordTemplate
        {
            get
            {
                if (string.IsNullOrEmpty(_forgotPasswordTemplate))
                {
                    _forgotPasswordTemplate = File.ReadAllText(ConfigHelper.EmailTemplatesPath + "ForgotPassword.htm");
                }
                return _forgotPasswordTemplate;
            }
        }

        public static string EmailVerificationTemplate
        {
            get
            {
                if (string.IsNullOrEmpty(_emailVerificationTemplate))
                {
                    _emailVerificationTemplate =
                        File.ReadAllText(ConfigHelper.EmailTemplatesPath + "EmailVerification.htm");
                }
                return _emailVerificationTemplate;
            }
        }

        public static string RequestDealsTemplate
        {
            get
            {
                if (string.IsNullOrEmpty(_requestDealsTemplate))
                {
                    _requestDealsTemplate = File.ReadAllText(ConfigHelper.EmailTemplatesPath + "RequestDeals.htm");
                }
                return _requestDealsTemplate;
            }
        }

        public static string RequestSubmittedTemplate
        {
            get
            {
                if (string.IsNullOrEmpty(_requestSubmittedTemplate))
                {
                    _requestSubmittedTemplate = File.ReadAllText(ConfigHelper.EmailTemplatesPath + "RequestSubmit.htm");
                }
                return _requestSubmittedTemplate;
            }
        }

        public static string TravellerTemplate
        {
            get
            {
                if (string.IsNullOrEmpty(_travellerTemplate))
                {
                    _travellerTemplate = File.ReadAllText(ConfigHelper.EmailTemplatesPath + "Traveller.htm");
                }
                return _travellerTemplate;
            }
        }

        #region Nested type: Keys

        public struct Keys
        {
            public static readonly string Key_User = "{user}";
            public static readonly string Key_Email = "{email}";
            public static readonly string Key_Password = "{password}";
            public static readonly string Key_EmailVerification_Url = "{email_verification_url}";
            public static readonly string Key_EmailCode = "{emailCode}";
            public static readonly string Key_DealsUrl = "{deals_url}";
            public static readonly string Key_Time = "{time}";
            public static readonly string Key_From = "{from}";
            public static readonly string Key_To = "{to}";
            public static readonly string Key_Date = "{date}";
            public static readonly string Key_Travellers = "{travellers}";
            public static readonly string Key_RequestFriendlyName = "{request_friendly_name}";
            public static readonly string Key_Traveller = "{traveller}";
            public static readonly string Key_BaseSiteUrl = "{BaseSiteUrl}";


            public static readonly string Key_Year = "{year}";
        }

        #endregion
    }
}