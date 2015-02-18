using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using TravelWithMe.API.Interfaces;
using TravelWithMe.API.AccountMgmt.Providers;

namespace TravelWithMe.API.Core.Factories
{
    public static class AccountProviderFactory
    {
        private static readonly ReaderWriterLockSlim Lock = new ReaderWriterLockSlim();

        private static IAccountProvider _provider;

        public static IAccountProvider GetAccountProvider()
        {
            Lock.EnterReadLock();
            try
            {
                {
                    if (_provider == null)
                    {
                        if (Configuration.IsMockProvider("Account"))
                            _provider = new MockAccountProvider(AuthenticationProviderFactory.GetAuthenticationProvider());
                        else
                            _provider = new AccountProvider(AuthenticationProviderFactory.GetAuthenticationProvider(),
                                                            CaptchaProviderFactory.GetCaptchaProvider(),
                                                            EmailProviderFactory.GetEmailProvider(),
                                                            MobileProviderFactory.GetMobileProvider());
                    }
                }
            }
            finally
            {
                Lock.ExitReadLock();
            }
            return _provider;
        }
    }
}
