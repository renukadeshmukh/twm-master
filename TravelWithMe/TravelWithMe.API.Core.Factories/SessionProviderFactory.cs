using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using TravelWithMe.API.Interfaces;
using TravelWithMe.API.SessionMgmt;

namespace TravelWithMe.API.Core.Factories
{
    public class SessionProviderFactory
    {
        private static readonly ReaderWriterLockSlim Lock = new ReaderWriterLockSlim();

        private static ISessionProvider _provider = new SessionProvider();

        public static ISessionProvider GetSessionProvider()
        {
            return _provider;
        }
    }
}
