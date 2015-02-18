using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TravelWithMe.API.Core.Model;

namespace TravelWithMe.API.Interfaces
{
    public interface ICacheProvider
    {
        bool AddValue(string key, object value);

        bool Exists(string key);

        object GetValue(string key);

        bool Remove(string key);

        ConcurrentDictionary<string, List<BookedSeat>> GetBookingCache();

        ConcurrentDictionary<int, BusInfo> GetBusCache();

        List<City> GetCities();

        List<State> GetStates(string countryCode);

        List<Country> GetCountryCache();

        int GetCityId(string cityName);

        List<CitySearch> GetRecentSearches();

        void UpdateSearchHistory(City from, City to);

        ConcurrentDictionary<int, string> GetSeatMapCache();

        int GetCityPointId(CityPoint cityPoint);

        ConcurrentDictionary<int, CityPoint> GetCityPointCache();
    }
}
