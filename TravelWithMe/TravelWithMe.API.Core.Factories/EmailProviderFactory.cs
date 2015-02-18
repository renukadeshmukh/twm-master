using System.Threading;
using TravelWithMe.API.Interfaces;
using TravelWithMe.API.EmailMgmt.Providers;

namespace TravelWithMe.API.Core.Factories
{
    public static class EmailProviderFactory
    {
        private static readonly ReaderWriterLockSlim Lock = new ReaderWriterLockSlim();

        private static IEmailProvider _provider;

        public static IEmailProvider GetEmailProvider()
        {
            Lock.EnterReadLock();
            try
            {
                {
                    if (_provider == null)
                    {
                        _provider = new EmailProvider();
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