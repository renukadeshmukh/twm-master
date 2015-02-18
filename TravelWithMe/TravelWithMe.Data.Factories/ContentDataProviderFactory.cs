using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TravelWithMe.Data.Interfaces;
using TravelWithMe.Data.MySql;

namespace TravelWithMe.Data.Factories
{
    public static class ContentDataProviderFactory
    {
        private static readonly ContentDataProvider ContentDataProvider = new ContentDataProvider();

        public static IContentDataProvider CreateContentDataProvider()
        {
            return ContentDataProvider;
        }
    }
}
