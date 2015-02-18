using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TravelWithMe.API.Interfaces;
using TravelWithMe.API.BookingMgmt.Providers;

namespace TravelWithMe.API.Core.Factories
{
    public static class BookingProviderFactory
    {
        private static IBookingProvider _bookingProvider;

        public static IBookingProvider GetBusProvider()
        {
            try
            {
                if (_bookingProvider == null)
                {
                    if (Configuration.IsMockProvider("Booking"))
                        _bookingProvider = new MockBookingProvider();
                    else
                        _bookingProvider = new BookingProvider();
                }
            }
            catch (Exception ex)
            {

            }
            return _bookingProvider;
        }
    }
}
