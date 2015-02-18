using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TravelWithMe.API.Core.Caching.Http;
using TravelWithMe.API.Interfaces;
using TravelWithMe.API.Core.Model;
using System.Web;
using System.Web.Caching;
using TravelWithMe.API.AccountMgmt.Providers;


namespace TravelWithMe.API.BusMgmt.Providers
{
    public class MockBusProvider : IBusProvider
    {
        #region IBusProvider members
        public string _busKey = "Buses";
        public string _cp = "Cp";
        public string _amenities = "Amenities";
        public string _rates = "Rates";
        public string _schedule = "BusSchedule";
        private string _seatMapCacheKey = "SeatMapCache";

        private readonly IAuthenticationProvider _authenticationProvider;

        public MockBusProvider(IAuthenticationProvider authenticationProvider)
        {
            _authenticationProvider = authenticationProvider;
            var buses = HttpRuntime.Cache[_busKey] as ConcurrentDictionary<int, BusInfo>;
            var cityPoints = HttpRuntime.Cache[_cp] as ConcurrentDictionary<int, ConcurrentDictionary<int, CityPoint>>;
            var amenities = HttpRuntime.Cache[_busKey] as ConcurrentDictionary<int, List<string>>;
            var rates = HttpRuntime.Cache[_rates] as ConcurrentDictionary<int, ConcurrentDictionary<int, BusRate>>;
            var schedule = HttpRuntime.Cache[_schedule] as ConcurrentDictionary<int, BusSchedule>;
            var seatMapCache = HttpRuntime.Cache[_schedule] as ConcurrentDictionary<int, string>;
            if (buses == null || buses.Count ==0)
            {
                #region add buses to cache
                buses = new ConcurrentDictionary<int, BusInfo>();
                List<BusInfo> busList = MockData.GetMockBuses();
                foreach (var item in busList)
                    buses[item.BusTripId] = item;
                HttpRuntime.Cache.Insert(_busKey, buses, null, Cache.NoAbsoluteExpiration,
                                         DateTime.Now.AddHours(1) - DateTime.Now,
                                         CacheItemPriority.Normal,
                                         null);
                #endregion
            }

            if (seatMapCache == null || seatMapCache.Count == 0)
            {
                #region add buses to cache
                seatMapCache = new ConcurrentDictionary<int, string>();
                List<SeatArrangement> seatMaps = MockData.GetDefaultSeatMaps();
                foreach (var item in seatMaps)
                    seatMapCache[item.Id] = item.SeatMap;
                HttpRuntime.Cache.Insert(_seatMapCacheKey, seatMapCache, null, Cache.NoAbsoluteExpiration,
                                         DateTime.Now.AddHours(1) - DateTime.Now,
                                         CacheItemPriority.Normal,
                                         null);
                #endregion
            }
            if (cityPoints == null || cityPoints.Count==0)
            {
                #region add cps to cache
                cityPoints = new ConcurrentDictionary<int, ConcurrentDictionary<int, CityPoint>>();

                foreach (var item in buses)
                {
                    ConcurrentDictionary<int, CityPoint> cpList = MockData.GetMockCityPoints(item.Value.FromLoc, item.Value.ToLoc);
                    cityPoints[item.Value.BusTripId] = cpList;
                }

                HttpRuntime.Cache.Insert(_cp, cityPoints, null, Cache.NoAbsoluteExpiration,
                                         DateTime.Now.AddHours(1) - DateTime.Now,
                                         CacheItemPriority.Normal,
                                         null);
                #endregion
            }

            if (amenities == null || amenities.Count==0)
            {
                #region add amenities to cache
                amenities = new ConcurrentDictionary<int, List<string>>();

                foreach (var item in buses)
                {
                    amenities[item.Value.BusTripId] = MockData.GetAmenities();
                }

                HttpRuntime.Cache.Insert(_amenities, amenities, null, Cache.NoAbsoluteExpiration,
                                         DateTime.Now.AddHours(1) - DateTime.Now,
                                         CacheItemPriority.Normal,
                                         null);
                #endregion
            }

            if (rates == null|| rates.Count==0)
            {
                #region add rates to cache
                rates = new ConcurrentDictionary<int, ConcurrentDictionary<int, BusRate>>();

                foreach (var item in buses)
                {
                    ConcurrentDictionary<int, BusRate> rateList = MockData.GetMockRates();
                    rates[item.Value.BusTripId] = rateList;
                }

                HttpRuntime.Cache.Insert(_rates, rates, null, Cache.NoAbsoluteExpiration,
                                         DateTime.Now.AddHours(1) - DateTime.Now,
                                         CacheItemPriority.Normal,
                                         null);
                #endregion
            }
            if (schedule == null||schedule.Count==0)
            {
                #region add schedule to cache
                schedule = new ConcurrentDictionary<int, BusSchedule>();

                foreach (var item in buses)
                {
                    schedule[item.Value.BusTripId] = MockData.GetMockSchedule();
                }

                HttpRuntime.Cache.Insert(_schedule, schedule, null, Cache.NoAbsoluteExpiration,
                                         DateTime.Now.AddHours(1) - DateTime.Now,
                                         CacheItemPriority.Normal,
                                         null);
                #endregion
            }

            foreach (int busTripId in buses.Keys)
            {
                BusInfo busInfo = buses[busTripId];
                busInfo.CityPoints = !cityPoints.ContainsKey(busTripId) ? null : cityPoints[busTripId].Values.ToList();
                busInfo.BusSchedule = !schedule.ContainsKey(busTripId) ? null : schedule[busTripId];
                busInfo.BusRates = !rates.ContainsKey(busTripId) ? null : rates[busTripId].Values.ToList();
                busInfo.Amenities = !amenities.ContainsKey(busTripId) ? null : amenities[busTripId];
            }
        }

        public int AddBus(Core.Model.BusInfo busInfo)
        {
            ConcurrentDictionary<int, BusInfo> buses = HttpRuntime.Cache[_busKey] as ConcurrentDictionary<int, BusInfo>;
            int maxId = buses.Keys.Max();
            busInfo.BusTripId = maxId+1;
            buses[busInfo.BusTripId] = busInfo;
            return busInfo.BusTripId;
        }

        public bool DeleteBus(int busTripId)
        {
            ConcurrentDictionary<int, BusInfo> buses = HttpRuntime.Cache[_busKey] as ConcurrentDictionary<int, BusInfo>;
            if (buses != null)
            {
                int key = buses.Where(x => x.Value.BusTripId == busTripId).Select(x => x.Key).First();
                if (!string.Equals(key, string.Empty))
                {
                    BusInfo busInfo = null;
                    buses.TryRemove(key, out busInfo);
                }
                return true;
            }
            return false;
        }

        public BusInfo GetBus(int busTripId)
        {
            BusInfo bus = GetBusFromBusTripId(busTripId);
            return bus;
        }

        public List<Core.Model.BusInfo> GetAllBuses(string busOperatorId)
        {
            ConcurrentDictionary<int, BusInfo> busesDict = HttpRuntime.Cache[_busKey] as ConcurrentDictionary<int, BusInfo>;
            List<BusInfo> buses = new List<BusInfo>();
            foreach (var item in busesDict)
            {
                if (string.Equals(item.Value.BusOperatorId, busOperatorId, StringComparison.InvariantCultureIgnoreCase))
                {

                    BusInfo bus = new BusInfo()
                                      {
                                          ArrivalTime = item.Value.ArrivalTime,
                                          BusName = item.Value.BusName,
                                          BusOperatorId = item.Value.BusOperatorId,
                                          BusTripId = item.Value.BusTripId,
                                          DepartureTime = item.Value.DepartureTime,
                                          FromLoc = item.Value.FromLoc,
                                          ToLoc = item.Value.ToLoc
                                      };
                    buses.Add(bus);
                }

            }
            return buses;
        }

        public bool UpdateBusDetails(BusInfo busInfo)
        {
            if (busInfo == null)
                return false;
            BusInfo bus = GetBusFromBusTripId(busInfo.BusTripId);
            if (bus == null)
                return false;
            bus.BusName = busInfo.BusName;
            bus.FromLoc = bus.FromLoc ?? new City();
            bus.FromLoc = UpdateLoc(busInfo.FromLoc.Name);
            bus.ToLoc = UpdateLoc(busInfo.ToLoc.Name);
            bus.DepartureTime = busInfo.DepartureTime;
            bus.ArrivalTime = busInfo.ArrivalTime;
            return true;
        }

        public BusSchedule GetBusSchedule(int busTripId)
        {
            BusSchedule busSch = GetBusScheduleFromBusTripId(busTripId);
            return busSch;
        }

        public bool UpdateBusScheduleDetails(int busTripId, BusSchedule busSchedule)
        {
            if (busSchedule == null)
                return false;
            BusSchedule busSch = GetBusScheduleFromBusTripId(busTripId);
            if (busSch == null)
                return false;
            ConcurrentDictionary<int, BusSchedule> schDict = HttpRuntime.Cache[_schedule] as ConcurrentDictionary<int, BusSchedule>;
            schDict[busTripId] = new BusSchedule(busSchedule);
            return true;
        }

        public bool UpdateAC(int busTripId, bool isAC)
        {
            BusInfo bus = GetBusFromBusTripId(busTripId);
            if (bus == null)
                return false;
            bus.IsAC = isAC;
            return true;
        }

        public bool SaveBusCityPoint(int bustripId, CityPoint cityPoint)
        {
            ConcurrentDictionary<int, CityPoint> cp = GetBusCityPointsFromTripId(bustripId);
            if (cityPoint == null)
                return false;
            if (cp == null)
                cp = new ConcurrentDictionary<int, CityPoint>();

            if(!cp.ContainsKey(cityPoint.CPId))
            {
                cityPoint.CPId = cp.Keys.Max() + 1;
            }
            cp[cityPoint.CPId] = new CityPoint(cityPoint);
            return true;
        }

        public bool DeleteBusCityPoint(int bustripId, int cpId)
        {
            ConcurrentDictionary<int, CityPoint> cp = GetBusCityPointsFromTripId(bustripId);
            if (cp == null || !cp.ContainsKey(cpId))
                return false;
            CityPoint cpoint = null;
            cp.TryRemove(cpId, out cpoint);
            return true;
        }

        public List<CityPoint> GetAllCityPoints(int bustripId)
        {
            ConcurrentDictionary<int, CityPoint> cp = GetBusCityPointsFromTripId(bustripId);
            if (cp != null && cp.Values != null)
            {
                return cp.Values.ToList();
            }
            return null;
        }

        public bool UpdateCityPoint(int bustripId, CityPoint cpt)
        {
            ConcurrentDictionary<int, CityPoint> cp = GetBusCityPointsFromTripId(bustripId);
            if (cp == null || !cp.ContainsKey(cpt.CPId))
                return false;

            cp[cpt.CPId] = new CityPoint(cpt);
               
            return true;
        }

        public bool DeleteBusRate(int bustripId, int busRateId)
        {
            ConcurrentDictionary<int, BusRate> br = GetBusRatesFromTripId(bustripId);
            if (br == null)
                return false;
            if (!br.ContainsKey((busRateId)))
                return false;
            BusRate rate = br[busRateId];
            if (rate == null)
                return false;
            br.TryRemove(busRateId, out rate);
            return true;
        }

        public int AddBusRate(int busTripId, BusRate busRate)
        {
            ConcurrentDictionary<int, BusRate> br = GetBusRatesFromTripId(busTripId);
            if (br == null)
                return 0;
            busRate.RateId = br.Keys.Max() + 1;
            br[busRate.RateId] = busRate;
            return busRate.RateId;
        }

        public BusRate GetBusRate(int busTripId, int busRateId)
        {
            ConcurrentDictionary<int, BusRate> br = GetBusRatesFromTripId(busTripId);
            if (br == null)
                return null;
            return br[busRateId];
        }

        public bool UpdateBusRate(int busTripId, BusRate busRate)
        {
            ConcurrentDictionary<int, BusRate> br = GetBusRatesFromTripId(busTripId);
            if (br == null || !br.ContainsKey(busRate.RateId))
                return false;
            br[busRate.RateId] = new BusRate(busRate);
            return true;
        }

        public List<BusRate> GetAllBusRate(int busTripId)
        {
            ConcurrentDictionary<int, BusRate> br = GetBusRatesFromTripId(busTripId);
            if (br == null)
                return null;
            return br.Values.ToList();
        }
        #endregion

        #region HelperMethods
        private DateTime IncrementDateTimeMinutes(int minutes)
        {
            DateTime dateTime = DateTime.Now.AddMinutes(minutes);
            return dateTime;
        }

        private BusInfo GetBusFromBusTripId(int busTripId)
        {
           
            ConcurrentDictionary<int, BusInfo> busesDict = HttpRuntime.Cache[_busKey] as ConcurrentDictionary<int, BusInfo>;
            BusInfo bus = busesDict.Values.Where(x => string.Equals(x.BusTripId, busTripId)).First();
            return bus;
        }

        private BusSchedule GetBusScheduleFromBusTripId(int busTripId)
        {
            var schDict = HttpRuntime.Cache[_schedule] as ConcurrentDictionary<int, BusSchedule>;
            if (schDict.ContainsKey(busTripId))
                return schDict[busTripId];
            else
                return null;
        }

        private ConcurrentDictionary<int, CityPoint> GetBusCityPointsFromTripId(int busTripId)
        {
            var cps = HttpRuntime.Cache[_cp] as ConcurrentDictionary<int, ConcurrentDictionary<int, CityPoint>>;
            if (cps == null)
                return null;
            ConcurrentDictionary<int, CityPoint> cpDict = cps[busTripId];
            return cpDict;
        }

        private ConcurrentDictionary<int, BusRate> GetBusRatesFromTripId(int busTripId)
        {
            var br = HttpRuntime.Cache[_rates] as ConcurrentDictionary<int, ConcurrentDictionary<int, BusRate>>;
            if (br == null)
                return null;
            ConcurrentDictionary<int, BusRate> brDict = br[busTripId];
            return brDict;
        }

        public City UpdateLoc(string cityName)
        {
            List<City> cities = new HttpRuntimeCache().GetCities();
            return cities.Find(c => string.Equals(c.Name, cityName));
        }
        #endregion

        public void UpdateSeatMap(int busTripId, int seatMapId)
        {
            ConcurrentDictionary<int, string> seatMaps = HttpRuntime.Cache[_seatMapCacheKey] as ConcurrentDictionary<int, string>;
            BusInfo bus = GetBusFromBusTripId(busTripId);
            bus.SeatMapId = seatMapId;
        }

        public int AddSeatmap(string seatMap)
        {
            ConcurrentDictionary<int, string> seatMaps = HttpRuntime.Cache[_seatMapCacheKey] as ConcurrentDictionary<int, string>;
            int maxId = seatMaps.Max(s => s.Key);
            seatMaps[maxId++] = seatMap;
            return maxId;
        }


        public bool SetBusPublishStatus(int busTripId, bool isEnabled)
        {
            BusInfo bus = GetBusFromBusTripId(busTripId);
            bus.IsEnabled = isEnabled;
            return true;
        }

        public bool SetBusStatus(int busTripId, bool isPublished)
        {
            BusInfo bus = GetBusFromBusTripId(busTripId);
            bus.IsPublished = isPublished;
            return true;
        }
    }
}
