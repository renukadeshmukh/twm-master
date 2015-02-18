using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using TravelWithMe.API.Interfaces;
using TravelWithMe.API.BusMgmt.Providers;

namespace TravelWithMe.API.Core.Factories
{
    public static class BusProviderFactory
    {
        public static IBusProvider GetBusProvider()
        {
            IBusProvider _provider = null;
            if (Configuration.IsMockProvider("Bus"))
            {
                _provider = new MockBusProvider(AuthenticationProviderFactory.GetAuthenticationProvider());
            }
            else
            {
                _provider = new BusProvider();
            }
            return _provider;
        }
    }
}
