using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TravelWithMe.API.Core.Model
{
    public class BusRate
    {
        public int RateId { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public decimal WeekDayRate { get; set; }
        public decimal WeekEndRate { get; set; }

        public BusRate()
        {
        }

        public BusRate(BusRate busRate)
        {
            DateFrom = busRate.DateFrom;
            DateTo = busRate.DateTo;
            RateId = busRate.RateId;
            WeekDayRate = busRate.WeekDayRate;
            WeekEndRate = busRate.WeekEndRate;
        }
    }
}
