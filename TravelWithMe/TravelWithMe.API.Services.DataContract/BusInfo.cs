using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TravelWithMe.API.Core.Model;

namespace TravelWithMe.API.Services.DataContract
{
    public class BusInfo
    {
        public City FromLoc { get; set; }

        public City ToLoc { get; set; }

        public BusTime DepartureTime { get; set; }

        public BusTime ArrivalTime { get; set; }
        
        public string BusType { get; set; }
        
        public bool IsAC { get; set; }

        public int SeatMapId { get; set; }

        public SeatArrangement SeatArrangement { get; set; }

        public int BusTripId { get; set; }

        public string BusName { get; set; }

        public bool IsEnabled { get; set; }

        public bool IsPublished { get; set; }
    }
}
