using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TravelWithMe.Data.Interfaces;
using TravelWithMe.Data.MySql;

namespace TravelWithMe.Data.Factories
{
    public static class BusDataProviderFactory
    {
        private static readonly BusDataProvider BusDataProvider = new BusDataProvider();

        public static IBusDataProvider CreateBusDataProvider()
        {
            return BusDataProvider;
        }
    }
}
