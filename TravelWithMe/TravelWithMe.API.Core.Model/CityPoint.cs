using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TravelWithMe.API.Core.Model
{
    public class CityPoint
    {
        public int CPId { get; set; }

        public BusTime CPTime{get; set;}

        public decimal Lat { get; set; }

        public decimal Long { get; set; }

        public int CityId { get; set; }

        public string CityName { get; set; }

        public string CpName { get; set; }

        public bool IsDropOffPoint { get; set; }

        public bool IsPickupPoint { get; set; }

        public CityPoint()
        {
        }

        public CityPoint(CityPoint cp)
        {
            CPId = cp.CPId;
            CPTime = new BusTime(cp.CPTime);
            Lat = cp.Lat;
            Long = cp.Long;
            CityId = cp.CityId;
            CityName = cp.CityName;
            CpName = cp.CpName;
            IsDropOffPoint = cp.IsDropOffPoint;
            IsPickupPoint = cp.IsPickupPoint;
        }
    }
}
