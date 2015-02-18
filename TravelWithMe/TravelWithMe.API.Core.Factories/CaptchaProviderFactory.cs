using System.Threading;
using TravelWithMe.API.Interfaces;
using TravelWithMe.API.ReCaptcha.Providers;

namespace TravelWithMe.API.Core.Factories
{
    public static class CaptchaProviderFactory
    {
        private static readonly ReaderWriterLockSlim Lock = new ReaderWriterLockSlim();

        private static ICaptchaProvider _provider;

        public static ICaptchaProvider GetCaptchaProvider()
        {
            Lock.EnterReadLock();
            try
            {
                {
                    if (_provider == null)
                    {
                        _provider = new ReCaptchaProvider();
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