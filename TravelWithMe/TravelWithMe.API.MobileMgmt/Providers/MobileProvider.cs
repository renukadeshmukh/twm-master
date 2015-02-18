using System.Net;
using TravelWithMe.API.Interfaces;
using TravelWithMe.API.Core.Model;
using TravelWithMe.API.Core.Model.Utilities;
using TravelWithMe.Data.Interfaces;
using TravelWithMe.Data.Factories;

namespace TravelWithMe.API.MobileMgmt.Providers
{
    public class MobileProvider : IMobileProvider
    {
        private const string Type = "Mobile";

        #region IMobileProvider Members

        public void ResendMobileCode(Account account)
        {
            string verificationCode = VerificationCodeGenerator.GenerateNewVerificationCode();
            bool isSuccessful = false;
            if (!Configuration.IsMobileMock)
            {
                var helper = new RequestResponseBuilder();
                string url =
                    string.Format(
                        Configuration.MobileVerificationBaseUrl +
                        Configuration.MobileVerificationFormat,
                        account.AccountType.ToString(), verificationCode);

                WebRequest request = helper.CreateRequest<WebRequest>(url, null);
                string responseString;

                WebResponse response = helper.GetResponse(request, out responseString);

                if (response != null && !string.IsNullOrEmpty(responseString))
                {
                    isSuccessful = true;
                }
            }
            else
            {
                verificationCode = "123456";
                isSuccessful = true;
            }

            if (isSuccessful)
            {
                ICodeVerificationDataProvider codeVerificationDataProvider =
                    CodeVerificationDataProviderFactory.CreateCodeVerificationDataProvider();
                if (codeVerificationDataProvider.Remove(account.AccountId, Type))
                {
                    codeVerificationDataProvider.SaveNewCode(account.AccountId, verificationCode, Type);
                }
            }
        }

        public bool VerifyMobile(Account account, string verificationCode)
        {
            if (account != null)
            {
                ICodeVerificationDataProvider codeVerificationDataProvider =
                    CodeVerificationDataProviderFactory.CreateCodeVerificationDataProvider();

                if (codeVerificationDataProvider.IsValid(account.AccountId, verificationCode, Type))
                {
                    codeVerificationDataProvider.Remove(account.AccountId, Type);
                    return true;
                }
            }
            return false;
        }

        #endregion
    }
}