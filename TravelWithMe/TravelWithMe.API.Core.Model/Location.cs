using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TravelWithMe.API.Core.Model
{
    public class Location:ICloneable
    {
        public string CityName { get; set; }

        public decimal Latitude { get; set; }
        
        public decimal Longitude { get; set; }
        
        public string State { get; set; }

        public string CityCode { get; set; }

        public int CityId { get; set; }
   

        public object Clone()
        {
            return MemberwiseClone();
        }

        public Location()
        {
        }

        public Location(Location loc)
        {
            CityCode = loc.CityCode;
            Latitude = loc.Latitude;
            Longitude = loc.Longitude;
            State = loc.State;
            CityName = loc.CityName;
            CityId = loc.CityId;
        }
    }

}
