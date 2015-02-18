using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TravelWithMe.API.Core.Model
{
    public class City
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string StateCode { get; set; }

        public string GeoCode { get; set; }
    }
}
