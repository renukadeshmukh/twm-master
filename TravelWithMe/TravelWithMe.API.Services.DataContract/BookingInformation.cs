using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TravelWithMe.API.Services.DataContract
{
    public class BookingInformation
    {
        public long BookingId { get; set; }

        public BusItinerary SelectedItinerary { get; set; }

        public string TravelDate { get; set; }

        public List<int> SelectedSeats { get; set; }

        public decimal TotalAmount { get; set; }

        public List<Passenger> Passengers { get; set; }

        public string Email { get; set; }

        public string ContactNumber { get; set; }

        public CityPoint PickupPoint { get; set; }

        public CityPoint DropOffPoint { get; set; }

        public string TransactionId { get; set; }
    }
}
