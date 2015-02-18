using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TravelWithMe.API.Services.DataContract
{
    public class Location
    {
        public string CityName { get; set; }

        public decimal Latitude { get; set; }

        public decimal Longitude { get; set; }

        public string State { get; set; }

        public string CityCode { get; set; }
    }
}
