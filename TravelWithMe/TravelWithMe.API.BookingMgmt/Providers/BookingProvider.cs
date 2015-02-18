using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TravelWithMe.API.Interfaces;

namespace TravelWithMe.API.BookingMgmt.Providers
{
    public class BookingProvider:IBookingProvider
    {
        public List<Core.Model.BookingInformation> GetAllBookings()
        {
            throw new NotImplementedException();
        }
    }
}
