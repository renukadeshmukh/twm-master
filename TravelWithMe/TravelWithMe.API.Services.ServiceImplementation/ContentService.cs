using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Activation;
using System.Text;
using TravelWithMe.API.Core.Infra;
using TravelWithMe.API.Logging;
using TravelWithMe.API.Services.DataContract;
using TravelWithMe.API.Services.ServiceContract;
using TravelWithMe.API.Services.DataContract.Messages;
using TravelWithMe.API.Interfaces;
using TravelWithMe.API.Core.Factories;
using TravelWithMe.Logging.Helper;
using Model = TravelWithMe.API.Core.Model;

namespace TravelWithMe.API.Services.ServiceImplementation
{
    //[ServerMessageLogger]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class ContentService : IContentService
    {
        private const string Source = "ContentService";

        public GetCountriesResponse GetCountries(string sessionId)
        {
            throw new NotImplementedException();
        }

        public GetStatesResponse GetStates(string sessionId, string countryCode)
        {
            throw new NotImplementedException();
        }

        public GetCitiesResponse GetCities(string sessionId, string stateCode)
        {
            throw new NotImplementedException();
        }

        public List<string> SearchCity(string sessionId, string searchKey)
        {
            List<string> citiesFound = new List<string>();
            if (string.IsNullOrEmpty(searchKey))
            {
                return citiesFound;
            }
            using (new ApplicationContextScope(new ApplicationContext()))
            {
                ApplicationContext.SetSessionId(sessionId);
                string authId = string.Empty;
                try
                {
                    ICacheProvider provider = CacheProviderFactory.GetCacheProvider();
                    List<Model.City> cities = provider.GetCities();
                    if (cities != null)
                    {
                        cities = cities.Where(c => c.Name.ToLower().Contains(searchKey.ToLower())).ToList();
                        citiesFound = cities.Select(c => c.Name).ToList();
                    }
                }
                catch (Exception exception)
                {
                    Logger.LogException(exception, Source, "SearchCity", Severity.Critical);
                }
            }
            return citiesFound;
        }

        public List<string> SearchCityPoint(string sessionId, string searchKey)
        {
            List<string> citiesPointFound = new List<string>();
            if (string.IsNullOrEmpty(searchKey))
            {
                return citiesPointFound;
            }
            using (new ApplicationContextScope(new ApplicationContext()))
            {
                ApplicationContext.SetSessionId(sessionId);
                string authId = string.Empty;
                try
                {
                    ICacheProvider provider = CacheProviderFactory.GetCacheProvider();
                    List<Model.CityPoint> cps = provider.GetCityPointCache().Values.ToList();
                    if (cps != null)
                    {
                        cps = cps.Where(c => c.CpName.ToLower().Contains(searchKey.ToLower())).ToList();
                        citiesPointFound = cps.Select(c => c.CpName).ToList();
                    }
                }
                catch (Exception exception)
                {
                    Logger.LogException(exception, Source, "SearchCityPoint", Severity.Critical);
                }
            }
            return citiesPointFound;
        }

        public RecentSearchesResponse GetRecentSearches(string sessionId)
        {
            RecentSearchesResponse response = new RecentSearchesResponse();
            using (new ApplicationContextScope(new ApplicationContext()))
            {
                string busOperatorId = string.Empty;
                try
                {
                    ICacheProvider provider = CacheProviderFactory.GetCacheProvider();
                    var searches = provider.GetRecentSearches();
                    if (searches != null && searches.Count > 0)
                    {
                        response.RecentSearches = searches.ToDataContract();
                        response.IsSuccess = true;
                    }
                    else
                    {
                        response.IsSuccess = false;
                    }
                }
                catch (Exception exception)
                {
                    response.IsSuccess = false;
                    Logger.LogException(exception, Source, "GetRecentSearches", Severity.Normal);
                }
            }
            return response;
        }
    }
}
