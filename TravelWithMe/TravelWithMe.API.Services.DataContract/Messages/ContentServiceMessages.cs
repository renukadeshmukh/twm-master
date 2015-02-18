using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TravelWithMe.API.Services.DataContract.Messages
{
    public class GetCountriesResponse : BaseResponse
    {
        public List<Country> Countries { get; set; }
    }

    public class GetStatesResponse : BaseResponse
    {
        public List<State> States { get; set; }
    }

    public class GetCitiesResponse : BaseResponse
    {
        public List<City> Cities { get; set; }
    }

    public class SearchCityResponse : BaseResponse
    {
        public List<City> Cities { get; set; }
    }

    public class RecentSearchesResponse : BaseResponse
    {
        public List<CitySearch> RecentSearches { get; set; }
    }

    
}
