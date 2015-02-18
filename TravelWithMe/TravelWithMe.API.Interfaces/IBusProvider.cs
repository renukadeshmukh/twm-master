using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TravelWithMe.API.Core.Model;

namespace TravelWithMe.API.Interfaces
{
    public interface IBusProvider
    {
        int AddBus(BusInfo busInfo);

        bool DeleteBus(int busTripId);

        List<BusInfo> GetAllBuses(string busOperatorId);

        bool SetBusPublishStatus(int busTripId, bool isEnabled);

        bool SetBusStatus(int busTripId, bool isPublished);

        BusInfo GetBus(int busTripId);

        bool UpdateBusDetails(BusInfo busInfo);

        bool UpdateBusScheduleDetails(int busTripId, BusSchedule busSchedule);

        BusSchedule GetBusSchedule(int busTripId);
        
        void UpdateSeatMap(int busTripId, int seatMapId);

        int AddSeatmap(string seatMap);

        #region city point

        bool SaveBusCityPoint(int busTripId, CityPoint cityPoint);

        bool DeleteBusCityPoint(int busTripId, int cpId);

        List<CityPoint> GetAllCityPoints(int bustripId);
        #endregion

        #region bus rate
        bool DeleteBusRate(int bustripId, int busRateId);

        int AddBusRate(int busTripId, BusRate busRate);

        BusRate GetBusRate(int busTripId,int busRateId);

        List<BusRate> GetAllBusRate(int busTripId);

        bool UpdateBusRate(int busTripId, BusRate busRate);
        #endregion
    }
}
