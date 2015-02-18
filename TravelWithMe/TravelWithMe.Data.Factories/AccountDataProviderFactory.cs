using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TravelWithMe.Data.Interfaces;
using TravelWithMe.Data.MySql;

namespace TravelWithMe.Data.Factories
{
    public static class AccountDataProviderFactory
    {
        private static readonly AccountDataProvider AccountDataProvider = new AccountDataProvider();

        public static IAccountDataProvider CreateAccountDataProvider()
        {
            return AccountDataProvider;
        }
    }
}
