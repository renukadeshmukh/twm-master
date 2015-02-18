using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TravelWithMe.API.Core.Model
{
    public class BookedSeat
    {
        public int BusTripId { get; set; }

        public int SeatNumber { get; set; }

        public DateTime TravelDate { get; set; }

        public long BookingId { get; set; }
    }
}
