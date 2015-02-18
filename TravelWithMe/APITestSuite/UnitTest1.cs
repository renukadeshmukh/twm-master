using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TravelWithMe.API.Core.Model;

namespace APITestSuite
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            DateTime time = DateTime.Now.AddHours(4);
            DateTime time2 = DateTime.Now.AddDays(2);
            string dt = DateTime.Now.ToShortDateString();
            TimeSpan difference = time - time2; //create TimeSpan object
            int diff = difference.Days;
            string onlytime = time.ToString("hh:mmtt");


            // bool b=DateTime.TryParse("Feb 1, 2012",out time2);
            //DateTime dt = Convert.ToDateTime("Feb 21, 2012");
            //string dtStr = dt.ToString("MMM dd, yyyy");
            //string dtStr1 = dt.ToString("MMM d, yyyy");
            //  int daydiff = time.
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void TimeTest()
        {//1/16/2013
            string str1 = DateTime.MinValue.ToShortDateString() + " 08:00PM";
            string str6 = DateTime.MinValue.ToShortDateString() + " 08:30PM";
            string str2 = DateTime.MinValue.ToShortDateString() + " 09:45PM";
            string str3 = DateTime.MinValue.ToShortDateString() + " 04:10AM";
            string str4 = DateTime.MinValue.ToShortDateString() + " 06:20AM";
            string str5 = DateTime.MinValue.ToShortDateString() + " 10:30AM";
            DateTime dt1 = DateTime.ParseExact(str1, "d/M/yyyy hh:mmtt", null);
            DateTime dt2 = DateTime.ParseExact(str2, "d/M/yyyy hh:mmtt", null);
            DateTime dt3 = DateTime.ParseExact(str3, "d/M/yyyy hh:mmtt", null).AddDays(1);
            DateTime dt4 = DateTime.ParseExact(str4, "d/M/yyyy hh:mmtt", null).AddDays(1);
            DateTime dt5 = DateTime.ParseExact(str5, "d/M/yyyy hh:mmtt", null).AddDays(1);
            DateTime dt6 = DateTime.ParseExact(str6, "d/M/yyyy hh:mmtt", null).AddDays(0);

            var min1 = dt1.Subtract(dt1).TotalMinutes;
            var min6 = dt1.Subtract(dt6).TotalMinutes;
            var min2 = dt2.Subtract(dt1).TotalMinutes;
            var min3 = dt3.Subtract(dt1).TotalMinutes;
            var min4 = dt4.Subtract(dt1).TotalMinutes; 
            var min5 = dt5.Subtract(dt1).TotalMinutes;

            Assert.IsTrue(true);
        }

        [TestMethod]
        public void TimeTest1()
        {
            DateTime dateTime = DateTime.Now;
            string sdateTime = DateTime.Now.ToShortDateString()+ " 12:00:00 AM";
            DateTime dt = Convert.ToDateTime(sdateTime);
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void GetJourneyDuration()
        {
            BusTime bt1 = new BusTime()
                              {
                                  Days = 0,
                                  Hours = 8,
                                  Meridian = Meridian.PM,
                                  Minutes = 30
                              };
            BusTime bt2 = new BusTime()
            {
                Days = 1,
                Hours = 10,
                Meridian = Meridian.AM,
                Minutes = 30
            };

            string diff = BusTime.GetJourneyTime(bt1, bt2);
            Assert.IsTrue(true);
        }
    }
}
