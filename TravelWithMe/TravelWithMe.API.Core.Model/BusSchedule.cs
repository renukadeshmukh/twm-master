using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TravelWithMe.API.Core.Model
{
    public class BusSchedule : ICloneable
    {
        public BusTripFrequency Frequency { get; set; }

        public List<DayOfWeek> Weekdays { get; set; }

        public List<DateRange> DateRanges { get; set; }

        public BusSchedule()
        {
        }

        public BusSchedule(BusSchedule bs)
        {
            Frequency = bs.Frequency;
           
            if (bs.DateRanges != null)
            {
                DateRanges = new List<DateRange>();
                DateRanges.AddRange(bs.DateRanges);
            }
           
            if(bs.Weekdays !=null)
            {
                Weekdays = new List<DayOfWeek>();
                Weekdays.AddRange(bs.Weekdays);
                
            }
           
        }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }

    public class DateRange
    {
        public int RangeId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }

        public DateRange()
        {
        }
        public DateRange(DateRange dr)
        {
            RangeId = dr.RangeId;
            FromDate = dr.FromDate;
            ToDate = dr.ToDate;
        }
    }

    public enum BusType
    {
        Seater,
        Sleeper,
        SemiSleeper
    }

    public enum BusTripFrequency
    {
        SpecificWeekDays,
        Daily,
        SpecificDates
    }
}
