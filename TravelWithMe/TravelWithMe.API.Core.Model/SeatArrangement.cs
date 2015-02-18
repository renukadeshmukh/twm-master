using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TravelWithMe.API.Core.Model
{
    public class SeatArrangement
    {
        public int Id { get; set; }

        public string SeatMap { get; set; }
    }

    public enum SeatArrangementType
    {
        TwoByTwo,
        TwoByOne
    }
}
