using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TravelWithMe.API.Services.DataContract
{
    public class CityPoint
    {
        public int CPId { get; set; }

        public BusTime CPTime { get; set; }

        public decimal Lat { get; set; }

        public decimal Long { get; set; }

        public int CityId { get; set; }

        public string CityName { get; set; }

        public string CPName { get; set; }

        public bool IsDropOffPoint { get; set; }

        public bool IsPickupPoint { get; set; }
    }
}
