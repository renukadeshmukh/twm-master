using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using TravelWithMe.API.Interfaces;
using TravelWithMe.API.InventoryMgmt.Providers;

namespace TravelWithMe.API.Core.Factories
{
    public class InventoryProviderFactory
    {
        private static readonly ReaderWriterLockSlim Lock = new ReaderWriterLockSlim();

        private static IInventoryProvider _provider;

        public static IInventoryProvider GetInventoryProvider()
        {
            Lock.EnterReadLock();
            try
            {
                {
                    if (_provider == null)
                    {
                        if (Configuration.IsMockProvider("Inventory"))
                            _provider = new MockInventoryProvider();
                        else
                            _provider = new InventoryProvider(CacheProviderFactory.GetCacheProvider());
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
