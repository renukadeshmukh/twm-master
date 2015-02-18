using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TravelWithMe.API.Services.DataContract
{
    public class BusTime
    {
        public int Hours { get; set; }

        public int Minutes { get; set; }

        public string Meridian { get; set; }

        public int Days { get; set; }

    }
}
