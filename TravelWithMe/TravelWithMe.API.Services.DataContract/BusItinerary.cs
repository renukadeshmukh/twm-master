using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TravelWithMe.API.Services.DataContract
{
    public class BusItinerary
    {
        public string BusOperatorId { get; set; }

        public string BusTripId { get; set; }

        public string OperatorName { get; set; }

        public string BusName { get; set; }

        public string BusType { get; set; }

        public string From { get; set; }

        public string To { get; set; }

        public BusTime DepartureTime { get; set; }

        public BusTime ArrivalTime { get; set; }

        public bool IsAC { get; set; }

        public List<CityPoint> CityPoints { get; set; }

        public string JourneryTime { get; set; }

        public int SeatsAvailable { get; set; }

        public decimal Fare { get; set; }

        public List<BookedSeat> BookedSeats { get; set; }

        public string SeatMap { get; set; }
    }
}
