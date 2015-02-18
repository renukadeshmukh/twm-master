using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using TravelWithMe.API.Core.Model;
using TravelWithMe.Data.Interfaces;
using TravelWithMe.Data.MySql.Driver;
using TravelWithMe.Data.MySql.Entities;
using TravelWithMe.Logging.Helper;

namespace TravelWithMe.Data.MySql
{
    public class ContentDataProvider : IContentDataProvider
    {
        private string Source = "ContentDataProvider";
        public List<Country> GetCountries()
        {
            List<Country> countries = new List<Country>();
            try
            {
                var db = new MySqlDatabase(DbConfiguration.DatabaseRead);
                MySqlCommand command = ContentCommandBuilder.CreateGetAllCountriesCommand(db.Connection);
                DataSet dataSet = db.ExecuteQuery(command);
                if(dataSet != null && dataSet.Tables[0] != null && dataSet.Tables[0].Rows!= null)
                {
                    foreach (DataRow row in dataSet.Tables[0].Rows)
                    {
                        Country country = new Country();
                        country.Code = row["CountryCode"].GetString();
                        country.Name = row["CountryName"].GetString();
                        countries.Add(country);
                    }
                }
                return countries;
            }
            catch (Exception ex)
            {
                DBExceptionLogger.LogException(ex, Source, "GetCountries", Severity.Critical);
                return null;
            }
        }

        public List<State> GetStatesByCountry(string countryCode)
        {
            List<State> states = new List<State>();
            try
            {
                var db = new MySqlDatabase(DbConfiguration.DatabaseRead);
                MySqlCommand command = ContentCommandBuilder.CreateGetStatesByCountryCommand(db.Connection, countryCode);
                DataSet dataSet = db.ExecuteQuery(command);
                if (dataSet != null && dataSet.Tables[0] != null && dataSet.Tables[0].Rows != null)
                {
                    foreach (DataRow row in dataSet.Tables[0].Rows)
                    {
                        State state = new State();
                        state.Code = row["StateCode"].GetString();
                        state.Name = row["StateName"].GetString();
                        states.Add(state);
                    }
                }
                return states;
            }
            catch (Exception ex)
            {
                DBExceptionLogger.LogException(ex, Source, "GetStatesByCountry", Severity.Critical);
                return null;
            }
        }

        public List<City> GetAllCities()
        {
            List<City> cities = new List<City>();
            try
            {
                var db = new MySqlDatabase(DbConfiguration.DatabaseRead);
                MySqlCommand command = ContentCommandBuilder.CreateGetAllCitiesCommand(db.Connection);
                DataSet dataSet = db.ExecuteQuery(command);
                if (dataSet != null && dataSet.Tables[0] != null && dataSet.Tables[0].Rows != null)
                {
                    foreach (DataRow row in dataSet.Tables[0].Rows)
                    {
                        var city = ParseCity(row);
                        cities.Add(city);
                    }
                }
                return cities;
            }
            catch (Exception ex)
            {
                DBExceptionLogger.LogException(ex, Source, "GetAllCities", Severity.Critical);
                return null;
            }
        }

        private static City ParseCity(DataRow row)
        {
            City city = new City();
            city.Id = row["CityId"].GetInt();
            city.Name = row["CityName"].GetString();
            city.StateCode = row["StateCode"].GetString();
            city.GeoCode = row["GeoCode"].GetString();
            return city;
        }


        public List<CitySearch> GetRecentSearches()
        {
            List<CitySearch> recentSearches = new List<CitySearch>();
            try
            {
                var db = new MySqlDatabase(DbConfiguration.DatabaseRead);
                MySqlCommand command = ContentCommandBuilder.CreateGetRecentSearchesCommand(db.Connection);
                DataSet dataSet = db.ExecuteQuery(command);
                if (dataSet != null && dataSet.Tables[0] != null && dataSet.Tables[0].Rows != null)
                {
                    foreach (DataRow row in dataSet.Tables[0].Rows)
                    {
                        CitySearch citySearch = ParseCitySearch(row);
                        recentSearches.Add(citySearch);
                    }
                }
                return recentSearches;
            }
            catch (Exception ex)
            {
                DBExceptionLogger.LogException(ex, Source, "GetRecentSearches", Severity.Critical);
                return null;
            }
        }

        public void UpdateSearchHistory(int fromCityId, int toCityId)
        {
            try
            {
                var db = new MySqlDatabase(DbConfiguration.DatabaseWrite);
                MySqlCommand cmd = ContentCommandBuilder.BuildUpdateSearchHistoryCommand(db.Connection, fromCityId, toCityId);
                db.ExecuteNonQuery(cmd);
            }
            catch (Exception ex)
            {
                DBExceptionLogger.LogException(ex, Source, "UpdateSearchHistory", Severity.Critical);
            }
        }

        private CitySearch ParseCitySearch(DataRow row)
        {
            CitySearch search = new CitySearch();
            search.From = new City();
            search.From.Id = row["FromCityId"].GetInt();
            search.From.Name = row["FromCity"].GetString();
            search.To = new City();
            search.To.Id = row["ToCityId"].GetInt();
            search.To.Name = row["ToCity"].GetString();
            search.SearchCount = row["Count"].GetInt();
            return search;
        }

        public int AddCity(string cityName)
        {
            try
            {
                int cityId;
                var db = new MySqlDatabase(DbConfiguration.DatabaseWrite);
                MySqlCommand cmd = ContentCommandBuilder.BuildAddCityCommand(db.Connection, cityName);
                db.ExecuteNonQuery(cmd, "pCityId", out cityId);
                return cityId;
            }
            catch (Exception ex)
            {
                DBExceptionLogger.LogException(ex, Source, "AddCity", Severity.Critical);
                return 0;
            }
        }
    }
}
