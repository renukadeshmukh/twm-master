using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TravelWithMe.API.Core.Model
{
    public class BusTime
    {
        public int Hours { get; set; }

        public int Minutes { get; set; }

        public Meridian Meridian { get; set; }

        public int Days { get; set; }

        public double MinutesSinceMidNight { get; set; }

        public BusTime()
        {
         
        }
        public BusTime(BusTime bt)
        {
            Days = bt.Days;
            Hours = bt.Hours;
            Meridian = bt.Meridian;
            Minutes = bt.Minutes;
            MinutesSinceMidNight = GetMinutesFromMidNight(bt);//bt.MinutesSinceMidNight;
        }

        private double GetMinutesFromMidNight(BusTime busTime)
        {
            string nightTimeStr = DateTime.Now.ToShortDateString() + " 12:00:00 AM";
            DateTime nightTime = Convert.ToDateTime(nightTimeStr);
            int meridianHour = 0;
            if (busTime.Meridian==Meridian.PM)
                meridianHour = 12;
            DateTime curTime = nightTime.AddDays(busTime.Days)
                                        .AddHours(busTime.Hours + meridianHour)
                                        .AddMinutes(busTime.Minutes);
            double minDiff = curTime.Subtract(nightTime).TotalMinutes;
            return minDiff;
        }

        public static string GetJourneyTime(BusTime start, BusTime end)
        {
            string nightTimeStr = DateTime.Now.ToShortDateString() + " 12:00:00 AM";
            DateTime nightTime = Convert.ToDateTime(nightTimeStr);
            int meridianHour = 0;
            if (start.Meridian == Meridian.PM)
                meridianHour = 12;
            else
                meridianHour = 0;
            DateTime startDate = nightTime.AddDays(start.Days)
                                        .AddHours(start.Hours + meridianHour)
                                        .AddMinutes(start.Minutes);
            if (end.Meridian == Meridian.PM)
                meridianHour = 12;
            else
                meridianHour = 0;
            DateTime endDate = nightTime.AddDays(end.Days)
                                        .AddHours(end.Hours + meridianHour)
                                        .AddMinutes(end.Minutes);
            TimeSpan timeDiff = endDate - startDate;
            string days = timeDiff.Days == 0 ? null : timeDiff.Days + "d";
            string hours = timeDiff.Hours == 0 ? null : timeDiff.Hours + "h";
            string min   = timeDiff.Minutes == 0 ? null : timeDiff.Minutes + "m";
            return string.Format("{0} {1} {2}", days ?? "", hours ?? "", min ?? "");
        }

        public override string ToString()
        {
            string time = string.Concat(Hours, '|',
                                        Minutes, '|',
                                        Meridian, '|',
                                        Days);
            return time;
        }
    }

    public enum Meridian
    {
        AM,
        PM
    }
}
