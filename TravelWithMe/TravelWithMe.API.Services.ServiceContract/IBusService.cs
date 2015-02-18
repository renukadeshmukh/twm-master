using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using TravelWithMe.API.Services.DataContract;

namespace TravelWithMe.API.Services.ServiceContract
{
    [ServiceContract(Name = "IBusServiceRest", Namespace = "http://www.busswitch.in/Services/2012/08")]
    public interface IBusService
    {
        [OperationContract]
        [WebInvoke(UriTemplate = "/bus/add/{authId}/{sessionId}",
            BodyStyle = WebMessageBodyStyle.Bare)]
        AddBusResponse AddBus(string authId, BusInfo busInfo, string sessionId);

        [OperationContract]
        [WebGet(UriTemplate = "/bus/SetBusStatus/{authId}/{bustripid}/{sessionId}?isEnabled={isEnabled}",
            BodyStyle = WebMessageBodyStyle.Bare)]
        SetBusStatusResponse SetBusStatus(string authId, string bustripid, string sessionId, string isEnabled);

        [OperationContract]
        [WebGet(UriTemplate = "/bus/SetBusPublishStatus/{authId}/{bustripid}/{sessionId}?isPublished={isPublished}",
            BodyStyle = WebMessageBodyStyle.Bare)]
        SetBusStatusResponse SetBusPublishStatus(string authId, string bustripid, string sessionId, string isPublished);

        [OperationContract]
        [WebGet(UriTemplate = "/bus/del/{authId}/{bustripid}/{sessionId}",
            BodyStyle = WebMessageBodyStyle.Bare)]
        DeleteBusResponse DeleteBus(string authId, string bustripid, string sessionId);

        [OperationContract]
        [WebGet(UriTemplate = "/bus/getall/{authId}/{sessionId}",
            BodyStyle = WebMessageBodyStyle.Bare)]
        GetAllBusesResponse GetAllBuses(string authId, string sessionId);

        [OperationContract]
        [WebInvoke(UriTemplate = "/bus/update/{authId}/{bustripId}/{sessionId}",
            BodyStyle = WebMessageBodyStyle.Bare)]
        UpdateBusDetailsResponse UpdateBusDetails(string authId, string bustripId, BusInfo busInfo, string sessionId);

        [OperationContract]
        [WebInvoke(UriTemplate = "/Update/frequency/{authId}/{busTripId}/{sessionId}",
            BodyStyle = WebMessageBodyStyle.Bare)]
        UpdateBusScheduleResponse UpdateBusScheduleDetails(string authId, string busTripId, BusSchedule busSchedule, string sessionId);

        [OperationContract]
        [WebGet(UriTemplate = "/get/frequency/{authId}/{busTripId}/{sessionId}",
            BodyStyle = WebMessageBodyStyle.Bare)]
        GetBusScheduleResponse GetBusScheduleDetails(string authId, string busTripId, string sessionId);

        [OperationContract]
        [WebGet(UriTemplate = "/bus/GetBusSeatMap/{authId}/{busTripId}/{sessionId}",
            BodyStyle = WebMessageBodyStyle.Bare)]
        GetSeatMapResponse GetBusSeatMap(string authId, string busTripId, string sessionId);

        [OperationContract]
        [WebGet(UriTemplate = "/bus/GetSeatMap/{authId}/{sessionId}/{seatMapId}",
            BodyStyle = WebMessageBodyStyle.Bare)]
        GetSeatMapResponse GetSeatMap(string authId, string sessionId, string seatMapId);

        [OperationContract]
        [WebInvoke(UriTemplate = "/UpdateSeatMap/{authId}/{sessionId}",
            BodyStyle = WebMessageBodyStyle.Bare)]
        UpdateSeatMapResponse UpdateSeatMap(string authId, UpdateSeatMapRQ seatMapRQ, string sessionId);

        [OperationContract]
        [WebGet(UriTemplate = "/bus/GetDefaultSeatMaps/{authId}/{sessionId}",
            BodyStyle = WebMessageBodyStyle.Bare)]
        GetDefaultSeatMapsResponse GetDefaultSeatMaps(string authId, string sessionId);

        //[OperationContract]
        //[WebGet(UriTemplate = "/Update/isAc/{authId}/{bustripId}/{isAc}",
        //    BodyStyle = WebMessageBodyStyle.Bare)]
        //UpdateACResponse UpdateAcDetails(string authId, string bustripId,string isAc);

        [OperationContract]
        [WebGet(UriTemplate = "/getBus/{authId}/{bustripId}/{sessionId}",
            BodyStyle = WebMessageBodyStyle.Bare)]
        GetBusResponse GetBus(string authId, string bustripId, string sessionId);

        [OperationContract]
        [WebInvoke(UriTemplate = "/cp/update/{authId}/{bustripId}/{sessionId}",
            BodyStyle = WebMessageBodyStyle.Bare)]
        UpdateCityPointResponse UpdateCityPoint(string authId, string bustripId, CityPoint cityPoint, string sessionId);
        
        [OperationContract]
        [WebGet(UriTemplate = "/cp/del/{authId}/{bustripId}/{cpId}/{sessionId}",
            BodyStyle = WebMessageBodyStyle.Bare)]
        DeleteCityPointResponse DeleteCityPoint(string authId, string bustripId, string cpId, string sessionId);

        [OperationContract]
        [WebGet(UriTemplate = "/cp/getAll/{authId}/{bustripId}/{sessionId}",
            BodyStyle = WebMessageBodyStyle.Bare)]
        GetAllCityPointResponse GetAllCityPoints(string authId, string bustripId, string sessionId);

        [OperationContract]
        [WebInvoke(
            UriTemplate =
                "/rate/add/{authId}/{bustripid}/{sessionId}",
            BodyStyle = WebMessageBodyStyle.Bare)]
        AddBusRateResponse AddBusRate(string authId, string bustripId, BusRate busRate, string sessionId);

        [OperationContract]
        [WebGet(UriTemplate = "/rate/delete/{authId}/{bustripid}/{busRateId}/{sessionId}",
            BodyStyle = WebMessageBodyStyle.Bare)]
        DeleteBusRateResponse DeleteBusRate(string authId, string bustripId, string busRateId, string sessionId);

        [OperationContract]
        [WebInvoke(UriTemplate = "/rate/update/{authId}/{bustripid}/{sessionId}",
            BodyStyle = WebMessageBodyStyle.Bare)]
        UpdateBusRateResponse UpdateBusRate(string authId, string bustripId, BusRate busRate, string sessionId);

        [OperationContract]
        [WebGet(
            UriTemplate =
                "/rate/GetAll/{authId}/{bustripid}/{sessionId}",
            BodyStyle = WebMessageBodyStyle.Bare)]
        GetAllBusRatesResponse GetAllBusRates(string authId, string bustripId, string sessionId);
    }
}
