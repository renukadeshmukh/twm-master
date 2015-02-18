using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TravelWithMe.API.Core.Model;

namespace TravelWithMe.Data.Interfaces
{
    public interface IBusDataProvider
    {
        #region BusInfo operations
        BusInfo GetBusInfo(int busTripId);

        int AddBusInfo(BusInfo busInfo);

        bool UpdateBusinfo(BusInfo busInfo);

        bool DeleteBusInfo(int busTripId);

        List<BusInfo> GetAllBusesForOperator(int operatorId);

        List<BusInfo> GetAllBuses();

        List<SeatArrangement> GetAllSeatMaps();

        bool SetBusPublishStatus(int busTripId, bool isPublished);

        bool SetBusStatus(int busTripId, bool isEnabled);

        void UpdateBusSeatMap(int busTripId, int seatMapId);

        int AddSeatMap(string seatMap);
        #endregion

        #region BusRate operations

        List<BusRate> GetAllBusRatesForBus(int bustripid);

        BusRate GetBusRate(int rateId);

        bool UpdateBusRate(BusRate busRate);

        bool DeleteBusRate(int rateId);

        bool DeleteAllRatesForBus(int bustripid);

        int AddBusRate(int bustripid, BusRate rate);


        #endregion

        #region CityPoint operations

        List<CityPoint> GetAllCityPointsForBus(int bustripid);

        List<CityPoint> GetAllCityPoint();

        int AddCityPoint(CityPoint cityPoint);

        bool SaveBusCityPoint(int busTripId, CityPoint cityPoint);

        bool DeleteBusCityPoint(int busTripId, int cpId);
        #endregion

        #region

        bool SaveBusFrequency(int busTripId, BusSchedule schedule);

        void DeleteScheduleDayOfWeeks(int busTripId);

        bool SaveScheduleDayOfWeek(int busTripId, DayOfWeek dayOfWeek);

        void DeleteScheduleDates(int busTripId);

        int SaveScheduleDate(int busTripId, DateRange dateRange);

        #endregion

    }
}
