using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TravelWithMe.Logging.Helper
{
    [Serializable]
    public class FlexigridObject
    {
        public List<string> cellNames = new List<string>();
        public int page;
        public List<FlexigridRow> rows = new List<FlexigridRow>();
        public int total;
    }

    [Serializable]
    public class FlexigridRow
    {
        public List<string> cell = new List<string>();
        public string id;
    }
}
