using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TravelWithMe.Data.MySql;
using TravelWithMe.Data.Interfaces;

namespace TravelWithMe.Data.Factories
{
    public static class CodeVerificationDataProviderFactory
    {
        private static readonly CodeVerificationDataProvider CodeVerificationDataProvider =
            new CodeVerificationDataProvider();

        public static ICodeVerificationDataProvider CreateCodeVerificationDataProvider()
        {
            return CodeVerificationDataProvider;
        }
    }
}
