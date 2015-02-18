using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TravelWithMe.API.Core.Model;

namespace TravelWithMe.API.Interfaces
{
    public interface IInventoryProvider
    {
        List<BusItinerary> SearchInventory(DateTime travelDate, int from, int to);

        BusItinerary GetSeatMap(string busTripId, DateTime dateTime, bool returnSeatMap);

        bool BookSeats(BookingInformation bookingInformation, out string errorMessage);

        BusItinerary GetBusItinerary(DateTime travelDate, string busTripId);

        List<BookingInformation> GetBusBookings(DateTime traveldate, int busTripId);

        bool UpdateBooking(BookingInformation bookingInformation, out string errorMessage);
        
        bool DeleteBooking(long bookingId);

        BookingInformation GetBooking(long bookingId);
    }
}
