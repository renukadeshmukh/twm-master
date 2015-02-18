using TravelWithMe.API.Interfaces;
using TravelWithMe.API.Core.Model;

namespace TravelWithMe.API.MobileMgmt.Providers
{
    public class MockMobileProvider : IMobileProvider
    {
        #region IMobileProvider Members

        public void ResendMobileCode(Account account)
        {
        }

        public bool VerifyMobile(Account account, string verificationCode)
        {
            if (verificationCode == "123456")
                return true;
            return false;
        }

        #endregion
    }
}