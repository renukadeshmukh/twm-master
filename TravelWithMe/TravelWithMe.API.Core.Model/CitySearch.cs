using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TravelWithMe.API.Core.Model
{
    public class CitySearch
    {
        public City From { get; set; }

        public City To { get; set; }

        public int SearchCount { get; set; }
    }
}
