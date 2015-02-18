using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TravelWithMe.API.Core.Model;

namespace TravelWithMe.API.Services.ServiceImplementation
{
    public static class Extensions
    {
        internal static List<string> GetWeekDays(List<DayOfWeek> list)
        {
            if (list == null)
                return null;
            List<string> weekdays=new List<string>();

            if (list != null || list.Count > 0)
            {
                foreach (var item in list)
                {
                    string day = item.ToString();
                    weekdays.Add(day.Substring(0,3));
                }
            }
            return weekdays;
        }

        internal static List<DayOfWeek> GetWeekDaysEnum(List<string> list)
        {
            if (list == null)
                return null;
            List<DayOfWeek> weekdays = new List<DayOfWeek>();
            if (list != null || list.Count > 0)
            {
                foreach (var item in list)
                    weekdays.Add(GetDayOfWeek(item));
            }
            return weekdays;
        }

        private static DayOfWeek GetDayOfWeek(string item)
        {
            switch (item)
            {
                case "Mon":
                    return DayOfWeek.Monday;
                case "Fri":
                    return DayOfWeek.Friday;
                case "Sat":
                    return DayOfWeek.Saturday;
                case "Sun":
                    return DayOfWeek.Sunday;
                case "Thu":
                    return DayOfWeek.Thursday;
                case "Tue":
                    return DayOfWeek.Tuesday;
                case "Wed":
                    return DayOfWeek.Wednesday;
            }
            throw new Exception("Week days are not valid!!!");
        }
        
        public static string GetTime(this DateTime dateTime, int noOfDays)
        {
            if (noOfDays == -1 || noOfDays==0)
                return dateTime.GetTFormattedTime();
            else if (noOfDays == 1)
                return string.Concat(dateTime.GetTFormattedTime(), " (next day)");
            else
                return string.Concat(dateTime.GetTFormattedTime(), " (day-", noOfDays, ")");
        }

        public static string GetTFormattedTime(this DateTime dateTime)
        {
            return dateTime.ToString("hh:mmtt");
        }

        public static int GetDays(this DateTime dateTime2,DateTime datetime1)
        {
            return Math.Abs(dateTime2.Subtract(datetime1).Days);
        }

        public static DateTime ParseStringToDateTime(this string datetime)
        {
            DateTime dt = Convert.ToDateTime(datetime);
            return dt;
        }

        public static List<CityPoint> SortCpOnBusTime(List<CityPoint> cps )
        {
            return new List<CityPoint>();
        }
    }
}
