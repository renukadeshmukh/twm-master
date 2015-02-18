using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TravelWithMe.API.Services.DataContract
{
    public class BusRate
    {
        public int RateId { get; set; }
        public string DateFrom { get; set; }
        public string DateTo { get; set; }
        public decimal WeekDayRate { get; set; }
        public decimal WeekEndRate { get; set; }

        public BusRate()
        {
        }

        public BusRate(BusRate busRate)
        {
            RateId = busRate.RateId;
            DateFrom = busRate.DateFrom;
            DateTo = busRate.DateTo;
            WeekDayRate = busRate.WeekDayRate;
            WeekEndRate = busRate.WeekEndRate;
        }
    }
}
