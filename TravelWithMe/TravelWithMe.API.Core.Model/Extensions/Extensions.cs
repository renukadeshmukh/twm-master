using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Globalization;


namespace TravelWithMe.API.Core.Model.Extensions
{
    public static class Extensions
    {
        #region Date Methods

        public static DateTime ToDate(this string item)
        {
            if (string.IsNullOrEmpty(item))
            {
                throw new InvalidCastException("String not a valid date.");
            }
            DateTime dateTime;

            if (DateTime.TryParseExact(item, "MM/dd/yyyy HH:mm", CultureInfo.InvariantCulture.DateTimeFormat,
                                       DateTimeStyles.AssumeLocal, out dateTime) ||
                DateTime.TryParseExact(item, "MM/dd/yyyy", CultureInfo.InvariantCulture.DateTimeFormat,
                                       DateTimeStyles.AssumeLocal, out dateTime))
            {
                return dateTime;
            }
            throw new InvalidCastException("String not a valid date.");
        }

        public static string ToDateTimeString(this DateTime item)
        {
            return item.ToString("MM/dd/yyyy HH:mm");
        }

        public static string ToDateString(this DateTime item)
        {
            return item.ToString("MM/dd/yyyy");
        }

        public static string ToFormattedDateString(this DateTime item)
        {
            return item.ToString("ddd, dd MMM");
        }

        public static string ToFormattedDateTimeString(this DateTime item)
        {
            return item.ToString("ddd, dd MMM - hh:mm tt");
        }

        #endregion

        #region RequestResponse

        public static string ExtractResponse(this WebException wex)
        {
            if (wex != null && wex.Response != null)
                using (var reader = new StreamReader(wex.Response.GetResponseStream()))
                {
                    string response = reader.ReadToEnd();
                    return string.IsNullOrEmpty(response) ? string.Empty : response;
                }
            return string.Empty;
        }

        public static bool Is<T>(this Type type)
        {
            return typeof(T).IsAssignableFrom(type);
        }

        #endregion
    }
}
