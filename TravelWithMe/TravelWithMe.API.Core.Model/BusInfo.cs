using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TravelWithMe.API.Core.Model
{
    public class BusInfo
    {
        public City FromLoc { get; set; }

        public City ToLoc { get; set; }

        public BusTime DepartureTime { get; set; }

        public BusTime ArrivalTime { get; set; }

        public int BusTripId { get; set; }
        
        public BusType BusType { get; set; }
        
        public bool IsAC { get; set; }
        
        public string BusOperatorId { get; set; }

        public string OperatorName { get; set; }

        public int SeatMapId { get; set; }
        
        public List<CityPoint> CityPoints { get; set; }
       
        public BusSchedule BusSchedule { get; set; }
        
        public string BusName { get; set; }

        public List<BusRate> BusRates { get; set; }

        public List<string> Amenities { get; set; }

        public bool IsEnabled { get; set; }

        public bool IsPublished { get; set; }

        public BusInfo()
        {
        }

        public BusInfo(string busOperatorId,BusInfo busInfo)
        {
            BusName = busInfo.BusName;
            FromLoc = busInfo.FromLoc;
            ToLoc = busInfo.ToLoc;
            ArrivalTime = new BusTime(busInfo.ArrivalTime);
            DepartureTime = new BusTime(busInfo.DepartureTime);
            BusOperatorId = busOperatorId;
            IsAC = busInfo.IsAC;
            BusType = busInfo.BusType;
        }

        public BusInfo(BusInfo busInfo)
        {
            BusName = busInfo.BusName;
            FromLoc = busInfo.FromLoc;
            ToLoc = busInfo.ToLoc;
            busInfo.ArrivalTime = new BusTime(busInfo.ArrivalTime);
            busInfo.DepartureTime = new BusTime(busInfo.DepartureTime);
            BusTripId = busInfo.BusTripId;
            BusOperatorId = busInfo.BusOperatorId;
            IsAC = busInfo.IsAC;
            BusType = busInfo.BusType;
        }
    }
}
