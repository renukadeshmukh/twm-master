using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TravelWithMe.API.Core.Model;

namespace TravelWithMe.Data.Interfaces
{
    public interface IInventoryDataProvider
    {
        List<BookedSeat> GetAllBookedSeats();

        bool SaveBookedSeat(BookedSeat bookedSeat, out string errorMessage);

        bool DeleteBookedSeats(long bookingId);

        bool SaveBookingInfo(BookingInformation bookingInfo, out string errorMessage);

        bool UpdateBookingInfo(BookingInformation bookingInfo, out string errorMessage);

        bool UpdatePassenger(Passenger passenger);

        bool SavePassenger(long bookingId, int accountId, Passenger passenger, out string errorMessage);

        List<BookingInformation> GetBusBookings(DateTime traveldate, int busTripId);
        
        bool DeleteBooking(long bookingId);

        BookingInformation GetBooking(long bookingId);
    }
}
