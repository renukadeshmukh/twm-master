using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TravelWithMe.API.Services.DataContract
{
    public class AddBusResponse : BaseResponse
    {
        public int BusTripId { get; set; }
    }

    public class GetBusResponse : BaseResponse
    {
        public BusInfo BusInfo { get; set; }
    }

    public class DeleteBusResponse : BaseResponse
    {
        
    }

    public class SetBusStatusResponse : BaseResponse
    {

    }

    public class AddBusRateResponse : BaseResponse
    {
        public int RateId { get; set; }
    }

    public class UpdateCityPointResponse : BaseResponse
    {
        public int CPId { get; set; }
    }

    public class DeleteCityPointResponse : BaseResponse
    {

    }

    public class GetAllCityPointResponse : BaseResponse
    {
        public List<CityPoint> CityPoints { get; set; }
    }

    public class GetAllBusesResponse : BaseResponse
    {
        public List<BusInfo> Buses { get; set; }
    }

    public class GetSeatMapResponse : BaseResponse
    {
        public string SeatMap { get; set; }
    }

    public class GetDefaultSeatMapsResponse : BaseResponse
    {
        public List<SeatArrangement> SeatMaps { get; set; }
    }

    public class UpdateBusDetailsResponse : BaseResponse
    {
    }

    public class UpdateBusScheduleResponse : BaseResponse
    {
    }

    public class GetBusScheduleResponse : BaseResponse
    {
        public BusSchedule BusSchedule { get; set; }
    }

    public class UpdateSeatMapRQ
    {
        public int BusTripId { get; set; }
        public int SeatMapId { get; set; }
        public string SeatMap { get; set; }
    }

    public class UpdateSeatMapResponse : BaseResponse
    {
        public bool UpdateStatus { get; set; }
    }

    public class UpdateACResponse : BaseResponse
    {
    }

    public class DeleteBusRateResponse : BaseResponse
    {
         
    }

    public class UpdateBusRateResponse : BaseResponse
    {
         
    }

    public class GetAllBusRatesResponse : BaseResponse
    {
        public List<BusRate> Rates { get; set; }
    }
}
