using System;
using TravelWithMe.API.Interfaces;

namespace TravelWithMe.API.AuthenticationMgmt.Providers
{
    public class AuthenticationProvider : IAuthenticationProvider
    {
        private readonly ICacheProvider _cacheProvider;

        public AuthenticationProvider(ICacheProvider cacheProvider)
        {
            _cacheProvider = cacheProvider;
        }

        #region IAuthenticationProvider Members

        public string CreateAuthenticationId(string accountId)
        {
            string authenticationId = Guid.NewGuid().ToString();

            _cacheProvider.AddValue(authenticationId, accountId);

            return authenticationId;
        }

        public bool Validate(string authenticationId)
        {
            return _cacheProvider.Exists(authenticationId);
        }

        public bool Logout(string authenticationId)
        {
            return _cacheProvider.Remove(authenticationId);
        }

        public string GetAccountId(string authenticationId)
        {
            return _cacheProvider.GetValue(authenticationId).ToString();
        }

        #endregion
    }
}