using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Web;
using TravelWithMe.API.Services.DataContract;
using TravelWithMe.API.Services.DataContract.Messages;

namespace TravelWithMe.API.Services.ServiceContract
{
    [ServiceContract(Name = "IAccountServiceRest", Namespace = "http://www.busswitch.in/Services/2012/08")]
    public interface IInventoryService
    {
        [OperationContract]
        [WebGet(
            UriTemplate =
                "/SearchBuses?session={sessionId}&traveldate={travelDate}&from={from}&to={to}",
            BodyStyle = WebMessageBodyStyle.Bare)]
        SearchBusesRS SearchBuses(string sessionId, string travelDate, string from, string to);

        [OperationContract]
        [WebGet(
            UriTemplate =
                "/GetBusBookings?session={sessionId}&authId={authId}&traveldate={travelDate}&busTripId={busTripId}",
            BodyStyle = WebMessageBodyStyle.Bare)]
        GetBusBookingsResponse GetBusBookings(string sessionId, string authId, string travelDate, string busTripId);

        [OperationContract]
        [WebGet(
            UriTemplate =
                "/GetSeatMap?session={sessionId}&busId={busTripId}&travelDate={travelDate}&returnSeatMap={returnSeatMap}",
            BodyStyle = WebMessageBodyStyle.Bare)]
        GetSeatMapRS GetSeatMap(string sessionId, string busTripId, string travelDate, bool returnSeatMap);

        [OperationContract]
        [WebInvoke(UriTemplate = "/BookSeats?session={sessionId}&authId={authId}",
            BodyStyle = WebMessageBodyStyle.Bare)]
        BookSeatsResponse BookSeats(string sessionId, string authId, BookingInformation bookingInfo);

        [OperationContract]
        [WebInvoke(UriTemplate = "/UpdateBooking?session={sessionId}&authId={authId}",
            BodyStyle = WebMessageBodyStyle.Bare)]
        UpdateBookingResponse UpdateBooking(string sessionId, string authId, BookingInformation bookingInfo);

        [OperationContract]
        [WebGet(
            UriTemplate =
                "/DeleteBooking?session={sessionId}&authId={authId}&busTripId={busTripId}&bookingId={bookingId}",
            BodyStyle = WebMessageBodyStyle.Bare)]
        DeleteBookingResponse DeleteBooking(string sessionId, string authId, string busTripId, string bookingId);

        [OperationContract]
        [WebGet(
            UriTemplate =
                "/GetBooking?session={sessionId}&authId={authId}&busTripId={busTripId}&bookingId={bookingId}",
            BodyStyle = WebMessageBodyStyle.Bare)]
        GetBookingResponse GetBooking(string sessionId, string authId, string busTripId, string bookingId);
    }
}
