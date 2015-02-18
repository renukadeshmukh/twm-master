using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TravelWithMe.API.Services.DataContract.Messages
{
    public class SearchBusesRS : BaseResponse
    {
        public List<BusItinerary> BusItineraries { get; set; }
    }

    public class GetSeatMapRS : BaseResponse
    {
        public string SeatMap { get; set; }

        public List<BookedSeat> BookedSeats { get; set; }

        public decimal Fare { get; set; }

        public List<CityPoint> CityPoints { get; set; }
    }

    public class BookSeatsResponse : BaseResponse
    {
        public List<BookedSeat> BookedSeats { get; set; }

        public List<int> SelectedSeats { get; set; }

        public long BookingId { get; set; }
    }

    public class GetBusBookingsResponse : BaseResponse
    {
        public List<BookingInformation> Bookings { get; set; }
    }

    public class UpdateBookingResponse : BaseResponse
    {
        public List<BookedSeat> BookedSeats { get; set; }

        public List<int> SelectedSeats { get; set; }
    }

    public class DeleteBookingResponse : BaseResponse
    {
        
    }

    public class GetBookingResponse : BaseResponse
    {
        public BookingInformation Booking { get; set; }
    }
}
