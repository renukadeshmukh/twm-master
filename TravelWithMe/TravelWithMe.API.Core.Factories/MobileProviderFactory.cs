using System.Threading;
using TravelWithMe.API.Interfaces;
using TravelWithMe.API.MobileMgmt.Providers;


namespace TravelWithMe.API.Core.Factories
{
    public static class MobileProviderFactory
    {
        private static readonly ReaderWriterLockSlim Lock = new ReaderWriterLockSlim();

        private static IMobileProvider _provider;

        public static IMobileProvider GetMobileProvider()
        {
            Lock.EnterReadLock();
            try
            {
                {
                    if (_provider == null)
                    {
                        if (MobileMgmt.Configuration.IsMobileMock)
                            _provider = new MockMobileProvider();
                        else
                            _provider = new MobileProvider();
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