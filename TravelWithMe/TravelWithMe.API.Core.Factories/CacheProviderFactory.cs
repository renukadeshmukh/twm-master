using System.Threading;
using TravelWithMe.API.Interfaces;
using TravelWithMe.API.Core.Caching.Http;

namespace TravelWithMe.API.Core.Factories
{
    public class CacheProviderFactory
    {
        private static readonly ReaderWriterLockSlim Lock = new ReaderWriterLockSlim();

        private static ICacheProvider _provider;

        public static ICacheProvider GetCacheProvider()
        {
            Lock.EnterReadLock();
            try
            {
                {
                    if (_provider == null)
                    {
                        _provider = new HttpRuntimeCache();
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