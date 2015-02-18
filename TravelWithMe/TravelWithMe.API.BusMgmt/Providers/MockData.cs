using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TravelWithMe.API.Core.Caching.Http;
using TravelWithMe.API.Core.Model;
using System.Collections.Concurrent;

namespace TravelWithMe.API.BusMgmt.Providers
{
    public class MockData
    {
        public static List<City> fromcityList = new HttpRuntimeCache().GetCities();

        public static List<City> tocityList = new HttpRuntimeCache().GetCities();

        public static List<string> amenities = new List<string>() { "Television", "Charging Point", "Blanket", "Water Bottle" };
        public static List<bool> acnonac = new List<bool>() { true, false, true, false, true, true, false };
        public static List<BusSchedule> busFrequencies = new List<BusSchedule>()
        {
            new BusSchedule
            {
                Frequency= BusTripFrequency.Daily,
                Weekdays= null,
                DateRanges= null
            },
            new BusSchedule
            {
                Frequency= BusTripFrequency.SpecificWeekDays,
                Weekdays= new List<DayOfWeek>(){DayOfWeek.Monday,DayOfWeek.Saturday,DayOfWeek.Sunday},
                DateRanges= null
            },
            new BusSchedule
            {
                Frequency= BusTripFrequency.SpecificDates,
                Weekdays= null,
                DateRanges= new List<DateRange>()
                {
                    new DateRange{ RangeId= 1, FromDate= DateTime.Now, ToDate= DateTime.Now.AddMonths(1) },
                    new DateRange{ RangeId= 2, FromDate= DateTime.Now.AddMonths(1), ToDate= DateTime.Now.AddMonths(2) },
                    new DateRange{ RangeId= 3, FromDate= DateTime.Now.AddMonths(2), ToDate= DateTime.Now.AddMonths(3) }
                }
            }
        };
        public static List<int> berths = new List<int> { 0, 12, 30 };
        public static List<int> seats = new List<int> { 0, 34, 48, 38, 60 };

        public static T Random<T>(List<T> list)
            where T : class
        {
            Random rnd = new Random(int.Parse(Guid.NewGuid().ToString().Substring(0, 8), System.Globalization.NumberStyles.HexNumber));
            var randNum = rnd.Next(list.Count);
            return list[randNum];
        }

        public static int RandomIndx<T>(List<T> list) where T : struct
        {
            Random random = new Random(int.Parse(Guid.NewGuid().ToString().Substring(0, 8), System.Globalization.NumberStyles.HexNumber));
            return random.Next(list.Count);
        }

        public static BusType Random()
        {
            Array values = Enum.GetValues(typeof(BusType));
            Random random =
                new Random(int.Parse(Guid.NewGuid().ToString().Substring(0, 8),
                                     System.Globalization.NumberStyles.HexNumber));
            BusType randomBar = (BusType)values.GetValue(random.Next(values.Length));
            return randomBar;
        }

        public static SeatArrangementType RandomSAT()
        {
            Array values = Enum.GetValues(typeof(SeatArrangementType));
            Random random = new Random(int.Parse(Guid.NewGuid().ToString().Substring(0, 8), System.Globalization.NumberStyles.HexNumber));
            SeatArrangementType randomBar = (SeatArrangementType)values.GetValue(random.Next(values.Length));
            return randomBar;
        }

        public static List<string> GetAmenities()
        {
            List<int> lst = new List<int>();
            for (int i = 0; i < 2; i++)
            {
                Random random = new Random(int.Parse(Guid.NewGuid().ToString().Substring(0, 8), System.Globalization.NumberStyles.HexNumber));
                int val = random.Next(amenities.Count);
                if (lst.Contains(val) == false)
                    lst.Add(val);
                else i--;
            }
            return new List<string>() { amenities[lst[0]], amenities[lst[1]] };
        }

        public List<int> Rand<T>(List<T> list, int cnt) where T : struct
        {
            List<int> retList = new List<int>();
            for (int i = 0; i < cnt; i++)
            {
                int val = RandomIndx(list);
                if (retList.Contains(val) == false)
                    retList.Add(val);
                else i--;

            }
            return retList;

        }
        public static List<BusInfo> GetMockBuses()
        {
            List<BusInfo> buses = new List<BusInfo>();
            for (int i = 1; i < 200; i++)
            {
                City from = (City) Random(fromcityList);
                City to = (City) Random(tocityList);
                DateTime dt = DateTime.Now;
                BusInfo bus = new BusInfo()
                {
                    BusTripId = i,
                    BusOperatorId = "1",
                    OperatorName = "Operator" + i,
                    BusName = "Bus1" + i,
                    FromLoc = from,
                    ToLoc = to,
                    DepartureTime = new BusTime() { Days = 0, Hours = 8, Minutes = 30, Meridian = Meridian.PM, MinutesSinceMidNight = 0 },
                    ArrivalTime = new BusTime() { Days = 1, Hours = 10, Minutes = 30, Meridian = Meridian.AM, MinutesSinceMidNight = 870 },
                    BusType = Random(),
                    IsAC = acnonac[RandomIndx<bool>(acnonac)],
                    SeatMapId = Random(GetDefaultSeatMaps()).Id,
                    IsEnabled = true,
                    IsPublished = true
                };
                buses.Add(bus);
            }
            buses[0].ArrivalTime.Days = 1;
            buses[1].ArrivalTime.Days = 2;
            return buses;
        }

        public static ConcurrentDictionary<int, CityPoint> GetMockCityPoints(City from, City to)
        {
            ConcurrentDictionary<int, CityPoint> dict = new ConcurrentDictionary<int, CityPoint>();
            dict[1]= new CityPoint{ CPId= 1, CPTime= new BusTime(){Hours = 8,Minutes =30,Meridian = Meridian.PM,Days = 0,MinutesSinceMidNight = GetMinsFromMidNight(8,30,12,0)}, CpName= "Ganjgolai", IsDropOffPoint= false, IsPickupPoint= true, CityName= from.Name, CityId= from.Id,Lat=19.109M,Long=77.224M };
            dict[2]=new CityPoint{ CPId= 2, CPTime= new BusTime(){Hours = 9,Minutes =45,Meridian = Meridian.PM,Days = 0,MinutesSinceMidNight = GetMinsFromMidNight(9,45,12,0)}, CpName= "ShiwajiChawk", IsDropOffPoint= false, IsPickupPoint= true, CityName= from.Name, CityId= from.Id,Lat=19.109M ,Long=77.224M};
            dict[3]=new CityPoint{ CPId= 3, CPTime= new BusTime(){Hours = 11,Minutes =55,Meridian = Meridian.PM,Days = 0,MinutesSinceMidNight = GetMinsFromMidNight(11,55,12,0)}, CpName= "5Number", IsDropOffPoint= false, IsPickupPoint= true, CityName= from.Name, CityId= from.Id,Lat=19.109M ,Long=77.224M};
            dict[4]=new CityPoint{ CPId= 4, CPTime= new BusTime(){Hours = 4,Minutes =10,Meridian = Meridian.AM,Days = 1,MinutesSinceMidNight = GetMinsFromMidNight(4,30,10,1)}, CpName= "Hadapsar", IsDropOffPoint= true, IsPickupPoint= false, CityName= to.Name, CityId= to.Id,Lat=19.109M ,Long=77.224M};
            dict[5]=new CityPoint{ CPId= 5, CPTime= new BusTime(){Hours = 6,Minutes =20,Meridian = Meridian.AM,Days = 1,MinutesSinceMidNight = GetMinsFromMidNight(6,20,0,1)}, CpName= "Shiwaji Nagar", IsDropOffPoint= true, IsPickupPoint= false, CityName= to.Name, CityId= to.Id,Lat=19.109M ,Long=77.224M};
            dict[6] = new CityPoint { CPId = 6, CPTime = new BusTime() { Hours = 10, Minutes = 30, Meridian = Meridian.AM, Days = 1, MinutesSinceMidNight = GetMinsFromMidNight(10, 30, 0, 1) }, CpName = "Pimpari", IsDropOffPoint = true, IsPickupPoint = false, CityName = to.Name, CityId = to.Id, Lat = 19.109M, Long = 77.224M };
            return dict;
        }

        private static double GetMinsFromMidNight(int hours, int mins, int meridian, int days)
        {
            string nightTimeStr = DateTime.Now.ToShortDateString() + " 12:00:00 AM";
            DateTime nightTime = Convert.ToDateTime(nightTimeStr);
            int meridianHour = 0;

            DateTime curTime = nightTime.AddDays(days)
                                        .AddHours(hours + meridian)
                                        .AddMinutes(mins);
            double minDiff = curTime.Subtract(nightTime).TotalMinutes;
            return minDiff;

        }

        public static ConcurrentDictionary<int, BusRate> GetMockRates()
        {

            ConcurrentDictionary<int, BusRate> dict = new ConcurrentDictionary<int, BusRate>();
            dict[1] = new BusRate(){ RateId= 1, DateFrom= DateTime.Now.AddDays(-1),DateTo= DateTime.Now.AddMonths(1), WeekDayRate= 250, WeekEndRate= 300 };
            dict[2] = new BusRate() { RateId = 2, DateFrom = DateTime.Now.AddMonths(1), DateTo = DateTime.Now.AddMonths(2), WeekDayRate = 350, WeekEndRate = 400 };
            return dict;
        }

        internal static List<SeatArrangement> GetDefaultSeatMaps()
        {
            return new List<SeatArrangement>()
                       {
                           new SeatArrangement() { Id = 1, SeatMap = "{\"Name\":\"Sleeper\",\"BusType\":\"Sleeper\",\"SeatCount\":0,\"BerthCount\":36,\"IsSelected\":true,\"Decks\":[{\"DeckType\":\"lowerDeck\",\"Size\":\"2X1\",\"Seats\":[{\"SeatType\":\"availableBerthH\",\"Row\":0,\"Col\":0,\"Label\":\"B1\",\"SeatNumber\":1,\"Price\":600},{\"SeatType\":\"availableBerthH\",\"Row\":1,\"Col\":0,\"Label\":\"B2\",\"SeatNumber\":2,\"Price\":600},{\"SeatType\":\"availableBerthH\",\"Row\":3,\"Col\":0,\"Label\":\"B3\",\"SeatNumber\":3,\"Price\":600},{\"SeatType\":\"availableBerthH\",\"Row\":0,\"Col\":1,\"Label\":\"B4\",\"SeatNumber\":4,\"Price\":500},{\"SeatType\":\"availableBerthH\",\"Row\":1,\"Col\":1,\"Label\":\"B5\",\"SeatNumber\":5,\"Price\":500},{\"SeatType\":\"availableBerthH\",\"Row\":3,\"Col\":1,\"Label\":\"B6\",\"SeatNumber\":6,\"Price\":500},{\"SeatType\":\"availableBerthH\",\"Row\":0,\"Col\":2,\"Label\":\"B7\",\"SeatNumber\":7,\"Price\":400},{\"SeatType\":\"availableBerthH\",\"Row\":1,\"Col\":2,\"Label\":\"B8\",\"SeatNumber\":8,\"Price\":400},{\"SeatType\":\"availableBerthH\",\"Row\":3,\"Col\":2,\"Label\":\"B9\",\"SeatNumber\":9,\"Price\":400},{\"SeatType\":\"availableBerthH\",\"Row\":0,\"Col\":3,\"Label\":\"B10\",\"SeatNumber\":10,\"Price\":300},{\"SeatType\":\"availableBerthH\",\"Row\":1,\"Col\":3,\"Label\":\"B11\",\"SeatNumber\":11,\"Price\":300},{\"SeatType\":\"availableBerthH\",\"Row\":3,\"Col\":3,\"Label\":\"B12\",\"SeatNumber\":12,\"Price\":300},{\"SeatType\":\"availableBerthH\",\"Row\":0,\"Col\":4,\"Label\":\"B13\",\"SeatNumber\":13,\"Price\":200},{\"SeatType\":\"availableBerthH\",\"Row\":1,\"Col\":4,\"Label\":\"B14\",\"SeatNumber\":14,\"Price\":200},{\"SeatType\":\"availableBerthH\",\"Row\":3,\"Col\":4,\"Label\":\"B15\",\"SeatNumber\":15,\"Price\":200},{\"SeatType\":\"availableBerthH\",\"Row\":0,\"Col\":5,\"Label\":\"B16\",\"SeatNumber\":16,\"Price\":100},{\"SeatType\":\"availableBerthH\",\"Row\":1,\"Col\":5,\"Label\":\"B17\",\"SeatNumber\":17,\"Price\":100},{\"SeatType\":\"availableBerthH\",\"Row\":3,\"Col\":5,\"Label\":\"B18\",\"SeatNumber\":18,\"Price\":100}],\"LastAddedSeatNumber\":18},{\"DeckType\":\"upperDeck\",\"Size\":\"2X1\",\"Seats\":[{\"SeatType\":\"availableBerthH\",\"Row\":0,\"Col\":0,\"Label\":\"B19\",\"SeatNumber\":19,\"Price\":600},{\"SeatType\":\"availableBerthH\",\"Row\":1,\"Col\":0,\"Label\":\"B20\",\"SeatNumber\":20,\"Price\":600},{\"SeatType\":\"availableBerthH\",\"Row\":3,\"Col\":0,\"Label\":\"B21\",\"SeatNumber\":21,\"Price\":600},{\"SeatType\":\"availableBerthH\",\"Row\":0,\"Col\":1,\"Label\":\"B22\",\"SeatNumber\":22,\"Price\":500},{\"SeatType\":\"availableBerthH\",\"Row\":1,\"Col\":1,\"Label\":\"B23\",\"SeatNumber\":23,\"Price\":500},{\"SeatType\":\"availableBerthH\",\"Row\":3,\"Col\":1,\"Label\":\"B24\",\"SeatNumber\":24,\"Price\":500},{\"SeatType\":\"availableBerthH\",\"Row\":0,\"Col\":2,\"Label\":\"B25\",\"SeatNumber\":25,\"Price\":400},{\"SeatType\":\"availableBerthH\",\"Row\":1,\"Col\":2,\"Label\":\"B26\",\"SeatNumber\":26,\"Price\":400},{\"SeatType\":\"availableBerthH\",\"Row\":3,\"Col\":2,\"Label\":\"B27\",\"SeatNumber\":27,\"Price\":400},{\"SeatType\":\"availableBerthH\",\"Row\":0,\"Col\":3,\"Label\":\"B28\",\"SeatNumber\":28,\"Price\":300},{\"SeatType\":\"availableBerthH\",\"Row\":1,\"Col\":3,\"Label\":\"B29\",\"SeatNumber\":29,\"Price\":300},{\"SeatType\":\"availableBerthH\",\"Row\":3,\"Col\":3,\"Label\":\"B30\",\"SeatNumber\":30,\"Price\":300},{\"SeatType\":\"availableBerthH\",\"Row\":0,\"Col\":4,\"Label\":\"B31\",\"SeatNumber\":31,\"Price\":200},{\"SeatType\":\"availableBerthH\",\"Row\":1,\"Col\":4,\"Label\":\"B32\",\"SeatNumber\":32,\"Price\":200},{\"SeatType\":\"availableBerthH\",\"Row\":3,\"Col\":4,\"Label\":\"B33\",\"SeatNumber\":33,\"Price\":200},{\"SeatType\":\"availableBerthH\",\"Row\":0,\"Col\":5,\"Label\":\"B34\",\"SeatNumber\":34,\"Price\":100},{\"SeatType\":\"availableBerthH\",\"Row\":1,\"Col\":5,\"Label\":\"B35\",\"SeatNumber\":35,\"Price\":100},{\"SeatType\":\"availableBerthH\",\"Row\":3,\"Col\":5,\"Label\":\"B36\",\"SeatNumber\":36,\"Price\":100}],\"LastAddedSeatNumber\":36}]}"},
                           new SeatArrangement() { Id = 2, SeatMap = "{\"Name\":\"Volvo sleeper\",\"BusType\":\"Sleeper\",\"SeatCount\":9,\"BerthCount\":36,\"IsSelected\":true,\"Decks\":[{\"DeckType\":\"lowerDeck\",\"Size\":\"2X1\",\"Seats\":[{\"SeatType\":\"availableSeat\",\"Row\":0,\"Col\":0,\"Label\":\"S1\",\"SeatNumber\":1,\"Price\":600},{\"SeatType\":\"availableSeat\",\"Row\":1,\"Col\":0,\"Label\":\"S2\",\"SeatNumber\":2,\"Price\":600},{\"SeatType\":\"availableSeat\",\"Row\":3,\"Col\":0,\"Label\":\"S3\",\"SeatNumber\":3,\"Price\":600},{\"SeatType\":\"availableSeat\",\"Row\":0,\"Col\":1,\"Label\":\"S4\",\"SeatNumber\":4,\"Price\":500},{\"SeatType\":\"availableSeat\",\"Row\":1,\"Col\":1,\"Label\":\"S5\",\"SeatNumber\":5,\"Price\":500},{\"SeatType\":\"availableSeat\",\"Row\":3,\"Col\":1,\"Label\":\"S6\",\"SeatNumber\":6,\"Price\":500},{\"SeatType\":\"availableSeat\",\"Row\":0,\"Col\":2,\"Label\":\"S7\",\"SeatNumber\":7,\"Price\":400},{\"SeatType\":\"availableSeat\",\"Row\":1,\"Col\":2,\"Label\":\"S8\",\"SeatNumber\":8,\"Price\":400},{\"SeatType\":\"availableSeat\",\"Row\":3,\"Col\":2,\"Label\":\"S9\",\"SeatNumber\":9,\"Price\":400},{\"SeatType\":\"availableBerthH\",\"Row\":0,\"Col\":3,\"Label\":\"B10\",\"SeatNumber\":10,\"Price\":300},{\"SeatType\":\"availableBerthH\",\"Row\":1,\"Col\":3,\"Label\":\"B11\",\"SeatNumber\":11,\"Price\":300},{\"SeatType\":\"availableBerthH\",\"Row\":3,\"Col\":3,\"Label\":\"B12\",\"SeatNumber\":12,\"Price\":300},{\"SeatType\":\"availableBerthH\",\"Row\":0,\"Col\":4,\"Label\":\"B13\",\"SeatNumber\":13,\"Price\":200},{\"SeatType\":\"availableBerthH\",\"Row\":1,\"Col\":4,\"Label\":\"B14\",\"SeatNumber\":14,\"Price\":200},{\"SeatType\":\"availableBerthH\",\"Row\":3,\"Col\":4,\"Label\":\"B15\",\"SeatNumber\":15,\"Price\":200},{\"SeatType\":\"availableBerthH\",\"Row\":0,\"Col\":5,\"Label\":\"B16\",\"SeatNumber\":16,\"Price\":100},{\"SeatType\":\"availableBerthH\",\"Row\":1,\"Col\":5,\"Label\":\"B17\",\"SeatNumber\":17,\"Price\":100},{\"SeatType\":\"availableBerthH\",\"Row\":3,\"Col\":5,\"Label\":\"B18\",\"SeatNumber\":18,\"Price\":100},{\"SeatType\":\"availableBerthH\",\"Row\":0,\"Col\":6,\"Label\":\"B19\",\"SeatNumber\":19,\"Price\":100}],\"LastAddedSeatNumber\":19},{\"DeckType\":\"upperDeck\",\"Size\":\"2X1\",\"Seats\":[{\"SeatType\":\"availableBerthH\",\"Row\":0,\"Col\":0,\"Label\":\"B20\",\"SeatNumber\":20,\"Price\":800},{\"SeatType\":\"availableBerthH\",\"Row\":1,\"Col\":0,\"Label\":\"B21\",\"SeatNumber\":21,\"Price\":800},{\"SeatType\":\"availableBerthH\",\"Row\":3,\"Col\":0,\"Label\":\"B22\",\"SeatNumber\":22,\"Price\":800},{\"SeatType\":\"availableBerthH\",\"Row\":0,\"Col\":1,\"Label\":\"B23\",\"SeatNumber\":23,\"Price\":700},{\"SeatType\":\"availableBerthH\",\"Row\":1,\"Col\":1,\"Label\":\"B24\",\"SeatNumber\":24,\"Price\":700},{\"SeatType\":\"availableBerthH\",\"Row\":3,\"Col\":1,\"Label\":\"B25\",\"SeatNumber\":25,\"Price\":700},{\"SeatType\":\"availableBerthH\",\"Row\":0,\"Col\":2,\"Label\":\"B26\",\"SeatNumber\":26,\"Price\":600},{\"SeatType\":\"availableBerthH\",\"Row\":1,\"Col\":2,\"Label\":\"B27\",\"SeatNumber\":27,\"Price\":600},{\"SeatType\":\"availableBerthH\",\"Row\":3,\"Col\":2,\"Label\":\"B28\",\"SeatNumber\":28,\"Price\":600},{\"SeatType\":\"availableBerthH\",\"Row\":0,\"Col\":3,\"Label\":\"B29\",\"SeatNumber\":29,\"Price\":500},{\"SeatType\":\"availableBerthH\",\"Row\":1,\"Col\":3,\"Label\":\"B30\",\"SeatNumber\":30,\"Price\":500},{\"SeatType\":\"availableBerthH\",\"Row\":3,\"Col\":3,\"Label\":\"B31\",\"SeatNumber\":31,\"Price\":500},{\"SeatType\":\"availableBerthH\",\"Row\":0,\"Col\":4,\"Label\":\"B32\",\"SeatNumber\":32,\"Price\":400},{\"SeatType\":\"availableBerthH\",\"Row\":1,\"Col\":4,\"Label\":\"B33\",\"SeatNumber\":33,\"Price\":400},{\"SeatType\":\"availableBerthH\",\"Row\":3,\"Col\":4,\"Label\":\"B34\",\"SeatNumber\":34,\"Price\":400},{\"SeatType\":\"availableBerthH\",\"Row\":0,\"Col\":5,\"Label\":\"B35\",\"SeatNumber\":35,\"Price\":300},{\"SeatType\":\"availableBerthH\",\"Row\":1,\"Col\":5,\"Label\":\"B36\",\"SeatNumber\":36,\"Price\":300},{\"SeatType\":\"availableBerthH\",\"Row\":3,\"Col\":5,\"Label\":\"B37\",\"SeatNumber\":37,\"Price\":300},{\"SeatType\":\"availableBerthH\",\"Row\":0,\"Col\":6,\"Label\":\"B38\",\"SeatNumber\":38,\"Price\":200},{\"SeatType\":\"availableBerthH\",\"Row\":1,\"Col\":6,\"Label\":\"B39\",\"SeatNumber\":39,\"Price\":200},{\"SeatType\":\"availableBerthH\",\"Row\":3,\"Col\":6,\"Label\":\"B40\",\"SeatNumber\":40,\"Price\":200},{\"SeatType\":\"availableBerthH\",\"Row\":0,\"Col\":7,\"Label\":\"B41\",\"SeatNumber\":41,\"Price\":100},{\"SeatType\":\"availableBerthH\",\"Row\":1,\"Col\":7,\"Label\":\"B42\",\"SeatNumber\":42,\"Price\":100},{\"SeatType\":\"availableBerthH\",\"Row\":3,\"Col\":7,\"Label\":\"B43\",\"SeatNumber\":43,\"Price\":100}],\"LastAddedSeatNumber\":45}]}"},
                           new SeatArrangement() { Id = 3, SeatMap = "{\"Name\":\"Volvo Seater\",\"BusType\":\"Seater\",\"SeatCount\":40,\"BerthCount\":0,\"IsSelected\":true,\"Decks\":[{\"DeckType\":\"lowerDeck\",\"Size\":\"2X2\",\"Seats\":[{\"SeatType\":\"availableSeat\",\"Row\":0,\"Col\":0,\"Label\":\"S1\",\"SeatNumber\":1,\"Price\":1000},{\"SeatType\":\"availableSeat\",\"Row\":1,\"Col\":0,\"Label\":\"S2\",\"SeatNumber\":2,\"Price\":1000},{\"SeatType\":\"availableSeat\",\"Row\":2,\"Col\":0,\"Label\":\"S3\",\"SeatNumber\":3,\"Price\":1000},{\"SeatType\":\"availableSeat\",\"Row\":3,\"Col\":0,\"Label\":\"S4\",\"SeatNumber\":4,\"Price\":1000},{\"SeatType\":\"availableSeat\",\"Row\":0,\"Col\":1,\"Label\":\"S5\",\"SeatNumber\":5,\"Price\":900},{\"SeatType\":\"availableSeat\",\"Row\":1,\"Col\":1,\"Label\":\"S6\",\"SeatNumber\":6,\"Price\":900},{\"SeatType\":\"availableSeat\",\"Row\":2,\"Col\":1,\"Label\":\"S7\",\"SeatNumber\":7,\"Price\":900},{\"SeatType\":\"availableSeat\",\"Row\":3,\"Col\":1,\"Label\":\"S8\",\"SeatNumber\":8,\"Price\":900},{\"SeatType\":\"availableSeat\",\"Row\":0,\"Col\":2,\"Label\":\"S9\",\"SeatNumber\":9,\"Price\":800},{\"SeatType\":\"availableSeat\",\"Row\":1,\"Col\":2,\"Label\":\"S10\",\"SeatNumber\":10,\"Price\":800},{\"SeatType\":\"availableSeat\",\"Row\":2,\"Col\":2,\"Label\":\"S11\",\"SeatNumber\":11,\"Price\":800},{\"SeatType\":\"availableSeat\",\"Row\":3,\"Col\":2,\"Label\":\"S12\",\"SeatNumber\":12,\"Price\":800},{\"SeatType\":\"availableSeat\",\"Row\":0,\"Col\":3,\"Label\":\"S13\",\"SeatNumber\":13,\"Price\":700},{\"SeatType\":\"availableSeat\",\"Row\":1,\"Col\":3,\"Label\":\"S14\",\"SeatNumber\":14,\"Price\":700},{\"SeatType\":\"availableSeat\",\"Row\":2,\"Col\":3,\"Label\":\"S15\",\"SeatNumber\":15,\"Price\":700},{\"SeatType\":\"availableSeat\",\"Row\":3,\"Col\":3,\"Label\":\"S16\",\"SeatNumber\":16,\"Price\":700},{\"SeatType\":\"availableSeat\",\"Row\":0,\"Col\":4,\"Label\":\"S17\",\"SeatNumber\":17,\"Price\":600},{\"SeatType\":\"availableSeat\",\"Row\":1,\"Col\":4,\"Label\":\"S18\",\"SeatNumber\":18,\"Price\":600},{\"SeatType\":\"availableSeat\",\"Row\":2,\"Col\":4,\"Label\":\"S19\",\"SeatNumber\":19,\"Price\":600},{\"SeatType\":\"availableSeat\",\"Row\":3,\"Col\":4,\"Label\":\"S20\",\"SeatNumber\":20,\"Price\":600},{\"SeatType\":\"availableSeat\",\"Row\":0,\"Col\":5,\"Label\":\"S21\",\"SeatNumber\":21,\"Price\":500},{\"SeatType\":\"availableSeat\",\"Row\":1,\"Col\":5,\"Label\":\"S22\",\"SeatNumber\":22,\"Price\":500},{\"SeatType\":\"availableSeat\",\"Row\":2,\"Col\":5,\"Label\":\"S23\",\"SeatNumber\":23,\"Price\":500},{\"SeatType\":\"availableSeat\",\"Row\":3,\"Col\":5,\"Label\":\"S24\",\"SeatNumber\":24,\"Price\":500},{\"SeatType\":\"availableSeat\",\"Row\":0,\"Col\":6,\"Label\":\"S25\",\"SeatNumber\":25,\"Price\":400},{\"SeatType\":\"availableSeat\",\"Row\":1,\"Col\":6,\"Label\":\"S26\",\"SeatNumber\":26,\"Price\":400},{\"SeatType\":\"availableSeat\",\"Row\":2,\"Col\":6,\"Label\":\"S27\",\"SeatNumber\":27,\"Price\":400},{\"SeatType\":\"availableSeat\",\"Row\":3,\"Col\":6,\"Label\":\"S28\",\"SeatNumber\":28,\"Price\":400},{\"SeatType\":\"availableSeat\",\"Row\":0,\"Col\":7,\"Label\":\"S29\",\"SeatNumber\":29,\"Price\":300},{\"SeatType\":\"availableSeat\",\"Row\":1,\"Col\":7,\"Label\":\"S30\",\"SeatNumber\":30,\"Price\":300},{\"SeatType\":\"availableSeat\",\"Row\":2,\"Col\":7,\"Label\":\"S31\",\"SeatNumber\":31,\"Price\":300},{\"SeatType\":\"availableSeat\",\"Row\":3,\"Col\":7,\"Label\":\"S32\",\"SeatNumber\":32,\"Price\":300},{\"SeatType\":\"availableSeat\",\"Row\":0,\"Col\":8,\"Label\":\"S33\",\"SeatNumber\":33,\"Price\":200},{\"SeatType\":\"availableSeat\",\"Row\":1,\"Col\":8,\"Label\":\"S34\",\"SeatNumber\":34,\"Price\":200},{\"SeatType\":\"availableSeat\",\"Row\":2,\"Col\":8,\"Label\":\"S35\",\"SeatNumber\":35,\"Price\":200},{\"SeatType\":\"availableSeat\",\"Row\":3,\"Col\":8,\"Label\":\"S36\",\"SeatNumber\":36,\"Price\":200},{\"SeatType\":\"availableSeat\",\"Row\":0,\"Col\":9,\"Label\":\"S37\",\"SeatNumber\":37,\"Price\":100},{\"SeatType\":\"availableSeat\",\"Row\":1,\"Col\":9,\"Label\":\"S38\",\"SeatNumber\":38,\"Price\":100},{\"SeatType\":\"availableSeat\",\"Row\":2,\"Col\":9,\"Label\":\"S39\",\"SeatNumber\":39,\"Price\":100},{\"SeatType\":\"availableSeat\",\"Row\":3,\"Col\":9,\"Label\":\"S40\",\"SeatNumber\":40,\"Price\":100}],\"LastAddedSeatNumber\":40}]}"}
                       };
        }

        public static BusSchedule GetMockSchedule()
        {
            return new BusSchedule(Random(busFrequencies));
        }
    }
}
