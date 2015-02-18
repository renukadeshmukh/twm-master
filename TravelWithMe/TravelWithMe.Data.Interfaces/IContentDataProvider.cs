using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TravelWithMe.API.Core.Model;

namespace TravelWithMe.Data.Interfaces
{
    public interface IContentDataProvider
    {
        List<Country> GetCountries();

        List<State> GetStatesByCountry(string countryCode);

        List<City> GetAllCities();

        List<CitySearch> GetRecentSearches();

        void UpdateSearchHistory(int fromCityId, int toCityId);

        int AddCity(string cityName);
    }
}
