using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using TravelWithMe.API.Services.DataContract;

namespace TravelWithMe.API.Services.ServiceContract
{
    [ServiceContract(Name = "ISessionServiceRest", Namespace = "http://www.busswitch.in/Services/2012/08")]
    public interface ISessionService
    {
        [OperationContract]
        [WebGet(
            UriTemplate =
                "/CreateSession?session={sessionId}",
            BodyStyle = WebMessageBodyStyle.Bare)]
        CreateSessionResponse CreateSession(string sessionId);

        [OperationContract]
        [WebInvoke(
            UriTemplate =
                "/SaveBookingInfo?session={sessionId}&section={section}",
            BodyStyle = WebMessageBodyStyle.Bare)]
        SaveBookingInfoResponse SaveBookingInfo(string sessionId, string section, BookingInformation bookingInfo);

        [OperationContract]
        [WebGet(
            UriTemplate =
                "/GetBookingInfo?session={sessionId}",
            BodyStyle = WebMessageBodyStyle.Bare)]
        BookingInfoResponse GetBookingInfo(string sessionId);

        [OperationContract]
        [WebGet(
            UriTemplate =
                "/GetSession?session={sessionId}",
            BodyStyle = WebMessageBodyStyle.Bare)]
        GetSessionResponse GetSessionData(string sessionId);
    }
}
