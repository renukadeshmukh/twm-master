using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TravelWithMe.Data.Interfaces;
using TravelWithMe.Data.MySql;

namespace TravelWithMe.Data.Factories
{
    public static class InventoryDataProviderFactory
    {
        private static readonly InventoryDataProvider InventoryDataProvider = new InventoryDataProvider();

        public static IInventoryDataProvider CreateInventoryDataProvider()
        {
            return InventoryDataProvider;
        }
    }
}
