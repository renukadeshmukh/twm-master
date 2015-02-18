using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlTypes;
using TravelWithMe.API.Core.Model;

namespace TravelWithMe.Data.MySql
{
    public static class Extensions
    {
        public static string GetString(this object rowData)
        {
            return Convert.IsDBNull(rowData) ? null : Convert.ToString(rowData);
        }

        public static int GetInt(this object rowData)
        {
            return Convert.IsDBNull(rowData) ? -1 : Convert.ToInt32(rowData);
        }

        public static T GetEnum<T>(this object rowData, T defValue)
        {
            return Convert.IsDBNull(rowData) ? defValue : (T)Enum.Parse(typeof(T), Convert.ToString(rowData));
        }

        public static DateTime GetDate(this object rowData)
        {
            return (DateTime)(Convert.IsDBNull(rowData) ? SqlDateTime.Null : Convert.ToDateTime(rowData.GetString()));
        }

        public static decimal GetDecimal(this object rowData, decimal defValue = 0M)
        {
            return Convert.IsDBNull(rowData) ? defValue : Convert.ToDecimal(rowData);
        }

        public static bool GetBool(this object rowData)
        {
            return Convert.IsDBNull(rowData) ? false : Convert.ToBoolean(rowData);
        }

        public static BusTime GetBusTime(this object rowData)
        {
            string bustimeStr = rowData.GetString();
            if(!string.IsNullOrEmpty(bustimeStr))
            {
                string[] timeparts = bustimeStr.Split('|');
                if (timeparts.Length != 4) return null;

                try
                {
                    BusTime bt = new BusTime();
                    bt.Hours = Convert.ToInt32(timeparts[0]);
                    bt.Minutes = Convert.ToInt32(timeparts[1]);
                    bt.Meridian = (Meridian) Enum.Parse(typeof (Meridian), timeparts[2]);
                    bt.Days = Convert.ToInt32(timeparts[3]);
                    return bt;
                }catch(Exception e)
                {
                }
            }
            return null;
        }
    }
}
