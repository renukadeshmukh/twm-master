using System.Threading;
using TravelWithMe.API.Interfaces;
using TravelWithMe.API.AuthenticationMgmt.Providers;

namespace TravelWithMe.API.Core.Factories
{
    public static class AuthenticationProviderFactory
    {
        private static readonly ReaderWriterLockSlim Lock = new ReaderWriterLockSlim();

        private static IAuthenticationProvider _provider;

        public static IAuthenticationProvider GetAuthenticationProvider()
        {
            Lock.EnterReadLock();
            try
            {
                {
                    if (_provider == null)
                    {
                        if (Configuration.IsMockProvider("Authentication"))
                            _provider = new MockAuthenticationProvider();
                        else
                            _provider = new AuthenticationProvider(CacheProviderFactory.GetCacheProvider());
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