using System;
using TravelWithMe.API.Interfaces;

namespace TravelWithMe.API.AuthenticationMgmt.Providers
{
    public class MockAuthenticationProvider : IAuthenticationProvider
    {
        #region IAuthenticationProvider Members

        public string CreateAuthenticationId(string accountId)
        {
            string authenticationId = Guid.NewGuid().ToString();

            return authenticationId;
        }

        public bool Validate(string authenticationId)
        {
            return true;
        }

        public bool Logout(string authenticationId)
        {
            return true;
        }

        public string GetAccountId(string authenticationId)
        {
            return "1";
        }

        #endregion
    }
}