using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TravelWithMe.API.Services.DataContract
{
    public class BookedSeat
    {
        public int SeatNumber { get; set; }

        public long BookingId { get; set; }
    }
}
