using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TravelWithMe.API.Core.Model;
using TravelWithMe.API.Interfaces;
using TravelWithMe.Data.Factories;
using TravelWithMe.Data.Interfaces;

namespace TravelWithMe.API.BusMgmt.Providers
{
    public class BusProvider : IBusProvider
    {
        private IBusDataProvider _dataProvider = null;
        public BusProvider()
        {
            _dataProvider = BusDataProviderFactory.CreateBusDataProvider();
        }
        public int AddBus(Core.Model.BusInfo busInfo)
        {
            int busTripId = _dataProvider.AddBusInfo(busInfo);
            return busTripId;
        }

        public bool DeleteBus(int busTripId)
        {
            return _dataProvider.DeleteBusInfo(busTripId);
        }

        public List<Core.Model.BusInfo> GetAllBuses(string busOperatorId)
        {
            List<Core.Model.BusInfo> buses = _dataProvider.GetAllBusesForOperator(Convert.ToInt32(busOperatorId));
            return buses;
        }

        public Core.Model.BusInfo GetBus(int busTripId)
        {
            throw new NotImplementedException();
        }

        public bool UpdateBusDetails(Core.Model.BusInfo busInfo)
        {
            return _dataProvider.UpdateBusinfo(busInfo);
        }

        public bool UpdateBusScheduleDetails(int busTripId, Core.Model.BusSchedule busSchedule)
        {
            _dataProvider.SaveBusFrequency(busTripId, busSchedule);
            _dataProvider.DeleteScheduleDates(busTripId);
            _dataProvider.DeleteScheduleDayOfWeeks(busTripId);
            if (busSchedule.Frequency == BusTripFrequency.SpecificDates)
            {
                foreach (var dateRange in busSchedule.DateRanges)
                {
                    int rangeId = _dataProvider.SaveScheduleDate(busTripId, dateRange);
                    dateRange.RangeId = rangeId;
                }
            }
            if(busSchedule.Frequency == BusTripFrequency.SpecificWeekDays)
            {
                foreach (var dayOfWeek in busSchedule.Weekdays)
                {
                    _dataProvider.SaveScheduleDayOfWeek(busTripId, dayOfWeek);
                }
            }
            return true;
        }

        public Core.Model.BusSchedule GetBusSchedule(int busTripId)
        {
            throw new NotImplementedException();
        }

        public void UpdateSeatMap(int busTripId, int seatMapId)
        {
            _dataProvider.UpdateBusSeatMap(busTripId, seatMapId);
        }

        public int AddSeatmap(string seatMap)
        {
            return _dataProvider.AddSeatMap(seatMap);
        }

        public List<Core.Model.CityPoint> GetAllCityPoints(int bustripId)
        {
            throw new NotImplementedException();
        }

        public bool DeleteBusRate(int bustripId, int busRateId)
        {
            return _dataProvider.DeleteBusRate(busRateId);
        }

        public int AddBusRate(int busTripId, Core.Model.BusRate busRate)
        {
            return _dataProvider.AddBusRate(busTripId, busRate);
        }

        public Core.Model.BusRate GetBusRate(int busTripId, int busRateId)
        {
            throw new NotImplementedException();
        }

        public List<Core.Model.BusRate> GetAllBusRate(int busTripId)
        {
            List<BusRate> busRates =  _dataProvider.GetAllBusRatesForBus(busTripId);
            if(busRates == null)
                busRates = new List<BusRate>();
            return busRates;
        }

        public bool UpdateBusRate(int busTripId, Core.Model.BusRate busRate)
        {
            return _dataProvider.UpdateBusRate(busRate);
        }

        public bool SetBusPublishStatus(int busTripId, bool isPublished)
        {
            return _dataProvider.SetBusPublishStatus(busTripId, isPublished);
        }

        public bool SetBusStatus(int busTripId, bool isEnabled)
        {
            return _dataProvider.SetBusStatus(busTripId, isEnabled); 
        }

        public bool SaveBusCityPoint(int busTripId, CityPoint cityPoint)
        {
            return _dataProvider.SaveBusCityPoint(busTripId, cityPoint);
        }

        public bool DeleteBusCityPoint(int busTripId, int cpId)
        {
            return _dataProvider.DeleteBusCityPoint(busTripId, cpId);
        }
    }
}
