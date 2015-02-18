using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;
using TravelWithMe.API.Core.Model;
using TravelWithMe.API.Interfaces;
using TravelWithMe.Data.Factories;
using TravelWithMe.Data.Interfaces;

namespace TravelWithMe.API.Core.Caching.Http
{
    public class HttpRuntimeCache : ICacheProvider
    {
        #region ICacheProvider Members
        private string _busKey = "Buses";
        private string _bookingDataKey = "BookingDataCache";
        private string _countryDataKey = "CountryDataCache";
        private string _stateDataKey = "StateDataCache";
        private string _cityDataKey = "CityDataCache";
        private string _searchHistoryKey = "SearchHistoryCache";
        private string _seatMapCacheKey = "SeatMapCache";
        private string _cityPointCacheKey = "CityPointCache";

        public bool AddValue(string key, object value)
        {
            try
            {
                HttpRuntime.Cache.Add(key, value, null, Cache.NoAbsoluteExpiration, new TimeSpan(1, 0, 0),
                                      CacheItemPriority.Normal,
                                      null);
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        public bool Exists(string key)
        {
            try
            {
                return HttpRuntime.Cache[key] != null;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public object GetValue(string key)
        {
            try
            {
                return HttpRuntime.Cache[key];
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public bool Remove(string key)
        {
            try
            {
                return HttpRuntime.Cache.Remove(key) != null;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public ConcurrentDictionary<string, List<BookedSeat>> GetBookingCache()
        {
            ConcurrentDictionary<string, List<BookedSeat>> bookingCache = ApplicationCache.GetValue(_bookingDataKey) as ConcurrentDictionary<string, List<BookedSeat>>;
            if(bookingCache == null)
            {
                bookingCache = new ConcurrentDictionary<string, List<BookedSeat>>();
                IInventoryDataProvider dataProvider = InventoryDataProviderFactory.CreateInventoryDataProvider();
                List<BookedSeat> bookedSeats = dataProvider.GetAllBookedSeats();
                if (bookedSeats != null)
                {
                    foreach (BookedSeat bookedSeat in bookedSeats)
                    {
                        string busTripDateKey = string.Format("{0}:{1:MM/dd/yyyy}", bookedSeat.BusTripId,
                                                              bookedSeat.TravelDate);
                        if(!bookingCache.ContainsKey(busTripDateKey))
                        {
                            bookingCache[busTripDateKey] = new List<BookedSeat>();
                        }
                        bookingCache[busTripDateKey].Add(bookedSeat);
                    }
                    ApplicationCache.Insert(_bookingDataKey, bookingCache, null, Cache.NoAbsoluteExpiration,
                                         DateTime.Now.AddDays(30) - DateTime.Now, CacheItemPriority.Normal,
                                         null);
                }
            }
            return bookingCache;
        }

        public ConcurrentDictionary<int, string> GetSeatMapCache()
        {
            ConcurrentDictionary<int, string> seatMapCache = ApplicationCache.GetValue(_seatMapCacheKey) as ConcurrentDictionary<int, string>;
            if (seatMapCache == null)
            {
                seatMapCache = new ConcurrentDictionary<int, string>();
                IBusDataProvider dataProvider = BusDataProviderFactory.CreateBusDataProvider();
                List<SeatArrangement> seatmaps = dataProvider.GetAllSeatMaps();
                if (seatmaps != null)
                {
                    foreach (SeatArrangement seatmap in seatmaps)
                    {
                        seatMapCache[seatmap.Id] = seatmap.SeatMap;
                    }
                }
                ApplicationCache.Insert(_seatMapCacheKey, seatMapCache, null, Cache.NoAbsoluteExpiration,
                                         DateTime.Now.AddDays(30) - DateTime.Now, CacheItemPriority.Normal,
                                         null);
            }
            return seatMapCache ?? new ConcurrentDictionary<int, string>();
        }

        public ConcurrentDictionary<int, BusInfo> GetBusCache()
        {
            ConcurrentDictionary<int, BusInfo> busCache = ApplicationCache.GetValue(_busKey) as ConcurrentDictionary<int, BusInfo>;
            if (busCache == null)
            {
                busCache = new ConcurrentDictionary<int, BusInfo>();
                IBusDataProvider dataProvider = BusDataProviderFactory.CreateBusDataProvider();
                List<BusInfo> buses = dataProvider.GetAllBuses();
                if (buses != null)
                {
                    foreach (BusInfo bus in buses)
                    {
                        busCache[bus.BusTripId] = bus;
                    }
                }
                ApplicationCache.Insert(_busKey, busCache, null, Cache.NoAbsoluteExpiration,
                                         DateTime.Now.AddDays(30) - DateTime.Now, CacheItemPriority.Normal,
                                         null);
            }
            return busCache;
        }

        public ConcurrentDictionary<int, CityPoint> GetCityPointCache()
        {
            ConcurrentDictionary<int, CityPoint> cityPointCache = ApplicationCache.GetValue(_cityPointCacheKey) as ConcurrentDictionary<int, CityPoint>;
            if (cityPointCache == null)
            {
                cityPointCache = new ConcurrentDictionary<int, CityPoint>();
                IBusDataProvider dataProvider = BusDataProviderFactory.CreateBusDataProvider();
                List<CityPoint> cityPoints = dataProvider.GetAllCityPoint();
                if (cityPoints != null)
                {
                    foreach (CityPoint cp in cityPoints)
                    {
                        cityPointCache[cp.CPId] = cp;
                    }
                }
                ApplicationCache.Insert(_cityPointCacheKey, cityPointCache, null, Cache.NoAbsoluteExpiration,
                                         DateTime.Now.AddDays(30) - DateTime.Now, CacheItemPriority.Normal,
                                         null);
            }
            return cityPointCache;
        }

        public int GetCityPointId(CityPoint cityPoint)
        {
            var cityPoints = GetCityPointCache();
            CityPoint foundCityPoint = cityPoints.Values.ToList().Find(c => string.Equals(c.CpName, cityPoint.CpName, StringComparison.OrdinalIgnoreCase));
            if (foundCityPoint != null)
            {
                return foundCityPoint.CPId;
            }
            IBusDataProvider dataProvider = BusDataProviderFactory.CreateBusDataProvider();
            cityPoint.CPId = dataProvider.AddCityPoint(cityPoint);
            cityPoints[cityPoint.CPId] = cityPoint;
            return cityPoint.CPId;
        }

        public List<Country> GetCountryCache()
        {
            List<Country> countryCache = ApplicationCache.GetValue(_countryDataKey) as List<Country>;
            if (countryCache == null)
            {
                countryCache = new List<Country>();
                IContentDataProvider dataProvider = ContentDataProviderFactory.CreateContentDataProvider();
                List<Country> countries = dataProvider.GetCountries();
                if (countries != null)
                {
                    ApplicationCache.Insert(_countryDataKey, countries, null, Cache.NoAbsoluteExpiration,
                                         DateTime.Now.AddDays(30) - DateTime.Now, CacheItemPriority.Normal,
                                         null);
                }
            }
            return countryCache;
        }

        public List<State> GetStates(string countryCode)
        {
            ConcurrentDictionary<string, List<State>> stateDataCache = ApplicationCache.GetValue(_stateDataKey) as ConcurrentDictionary<string, List<State>>;
            if (stateDataCache == null)
            {
                stateDataCache = new ConcurrentDictionary<string, List<State>>();
                ApplicationCache.Insert(_stateDataKey, stateDataCache, null, Cache.NoAbsoluteExpiration,
                                        DateTime.Now.AddDays(30) - DateTime.Now, CacheItemPriority.Normal,
                                        null);
            }
            if(stateDataCache.ContainsKey(countryCode))
            {
                IContentDataProvider dataProvider = ContentDataProviderFactory.CreateContentDataProvider();
                List<State> states = dataProvider.GetStatesByCountry(countryCode);
                stateDataCache[countryCode] = states;
            }
            return stateDataCache[countryCode];
        }

        public List<City> GetCities()
        {
            List<City> cityCache = ApplicationCache.GetValue(_cityDataKey) as List<City>;
            if (cityCache == null)
            {
                cityCache = new List<City>();
                IContentDataProvider dataProvider = ContentDataProviderFactory.CreateContentDataProvider();
                cityCache = dataProvider.GetAllCities();
                if (cityCache != null)
                {
                    ApplicationCache.Insert(_countryDataKey, cityCache, null, Cache.NoAbsoluteExpiration,
                                         DateTime.Now.AddDays(30) - DateTime.Now, CacheItemPriority.Normal,
                                         null);
                }
            }
            return cityCache;
        }

        public int GetCityId(string cityName)
        {
            List<City> cities = GetCities();
            City foundCity = cities.Find(c => string.Equals(c.Name, cityName, StringComparison.OrdinalIgnoreCase));
            if(foundCity!=null)
            {
                return foundCity.Id;
            }
            IContentDataProvider contentDataProvider = ContentDataProviderFactory.CreateContentDataProvider();
            City city = new City();
            city.Name = cityName;
            city.Id = contentDataProvider.AddCity(cityName);
            cities.Add(city);
            return city.Id;
        }
        #endregion

        public List<CitySearch> GetRecentSearches()
        {
            List<CitySearch> searchHistory = ApplicationCache.GetValue(_searchHistoryKey) as List<CitySearch>;
            if (searchHistory == null)
            {
                IContentDataProvider dataProvider = ContentDataProviderFactory.CreateContentDataProvider();
                searchHistory = dataProvider.GetRecentSearches();
                if (searchHistory != null)
                {
                    ApplicationCache.Insert(_searchHistoryKey, searchHistory, null, Cache.NoAbsoluteExpiration,
                                         DateTime.Now.AddDays(30) - DateTime.Now, CacheItemPriority.Normal,
                                         null);
                }
            }

            if (searchHistory.Count < 5)
                return searchHistory.Select(c => c).OrderByDescending(search => search.SearchCount).ToList();
            
            return searchHistory.Select(c => c).OrderByDescending(search => search.SearchCount).ToList().GetRange(0, 5);

        }

        public void UpdateSearchHistory(City from, City to)
        {
            //Update the cache if this search is available
            List<CitySearch> citySearches = GetRecentSearches();
            CitySearch citySearch = citySearches.Find(c => c.From.Id == from.Id && c.To.Id == to.Id);
            if (citySearch != null)
            {
                citySearch.SearchCount++;
            }
            else
            {
                citySearches.Add(new CitySearch() {From = from, To = to, SearchCount = 1});
            }
            //Update Search history into database
            IContentDataProvider dataProvider = ContentDataProviderFactory.CreateContentDataProvider();
            dataProvider.UpdateSearchHistory(from.Id, to.Id);
        }
    }
}