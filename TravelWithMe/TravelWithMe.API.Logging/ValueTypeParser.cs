using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace TravelWithMe.API.Logging
{
    public static class ValueTypeParser
    {
        public static T Parse<T>(string arg, T defaultValue) where T : struct
        {
            try
            {
                if (string.IsNullOrEmpty(arg))
                    return defaultValue;
                return (T)Convert.ChangeType(arg, typeof(T));
            }
            catch (Exception)
            {
                return defaultValue;
            }
        }

        public static T Parse<T>(string arg, T defaultValue, bool ignoreCase) where T : struct
        {
            try
            {
                if (string.IsNullOrEmpty(arg))
                    return defaultValue;
                Type dataType = typeof(T);
                if (dataType.BaseType == typeof(Enum))
                    return (T)Enum.Parse(dataType, arg, ignoreCase);
                return Parse(arg, defaultValue);
            }
            catch (Exception)
            {
                return defaultValue;
            }
        }

        public static DateTime? Parse(string arg, DateTime? defaultValue, string format)
        {
            DateTime returnValue;
            string[] formats = { "M/d/yyyy h:mm:ss tt", "MM/dd/yyyy" };
            if (DateTime.TryParseExact(arg, formats, null, DateTimeStyles.None, out returnValue) == false)
                return defaultValue;
            return returnValue;
        }

        private static DateTimeOffset? Parse(string value, DateTimeOffset? defaultValue, string format)
        {
            DateTimeOffset returnValue;
            string[] formats = { "M/d/yyyy h:mm:ss tt", "MM/dd/yyyy" };
            if (DateTimeOffset.TryParseExact(value, formats, null, DateTimeStyles.None, out returnValue) == false)
                return defaultValue;
            return returnValue;
        }
    }
}
