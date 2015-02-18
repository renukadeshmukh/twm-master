using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TravelWithMe.API.Services.DataContract
{
    public class BusSchedule
    {
        public string Frequency { get; set; }

        public List<string> Weekdays { get; set; }

        public List<DateRange> DateRanges { get; set; }
    }

    public class DateRange
    {
        public int RateId { get; set; }
        public string From { get; set; }
        public string To { get; set; }
    }
}
