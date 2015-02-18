using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using TravelWithMe.API.Services.DataContract;
using TravelWithMe.API.Services.DataContract.Messages;

namespace TravelWithMe.API.Services.ServiceContract
{
    [ServiceContract(Name = "IContentServiceRest", Namespace = "http://www.busswitch.in/Services/2012/08")]
    public interface IContentService
    {
        [OperationContract]
        [WebGet(
            UriTemplate =
                "/GetCountries?session={sessionId}",
            BodyStyle = WebMessageBodyStyle.Bare)]
         GetCountriesResponse GetCountries(string sessionId);

        [OperationContract]
        [WebGet(
            UriTemplate =
                "/GetStates?session={sessionId}&country={countryCode}",
            BodyStyle = WebMessageBodyStyle.Bare)]
        GetStatesResponse GetStates(string sessionId, string countryCode);

        [OperationContract]
        [WebGet(
            UriTemplate =
                "/GetCities?session={sessionId}&state={stateCode}",
            BodyStyle = WebMessageBodyStyle.Bare)]
        GetCitiesResponse GetCities(string sessionId, string stateCode);

        [OperationContract]
        [WebGet(
            UriTemplate =
                "/SearchCity?session={sessionId}&term={searchKey}",
            BodyStyle = WebMessageBodyStyle.Bare)]
        List<string> SearchCity(string sessionId, string searchKey);

        [OperationContract]
        [WebGet(
            UriTemplate =
                "/SearchCityPoint?session={sessionId}&term={searchKey}",
            BodyStyle = WebMessageBodyStyle.Bare)]
        List<string> SearchCityPoint(string sessionId, string searchKey);

        [OperationContract]
        [WebGet(
            UriTemplate =
                "/GetRecentSearches?session={sessionId}",
            BodyStyle = WebMessageBodyStyle.Bare)]
        RecentSearchesResponse GetRecentSearches(string sessionId);
    }
}
