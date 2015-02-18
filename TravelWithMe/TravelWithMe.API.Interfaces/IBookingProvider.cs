using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TravelWithMe.API.Core.Model;

namespace TravelWithMe.API.Interfaces
{
    public interface IBookingProvider
    {
        List<BookingInformation> GetAllBookings();
       
    }
}
