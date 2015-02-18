using System;
using System.Collections.Specialized;
using System.ServiceModel.Activation;
using System.Web;
using TravelWithMe.API.Logging;
using TripsErrorUI.Services;

namespace Logging.Services
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class ReportService : IReportService
    {
        #region IReportService Members

        public string GetExceptions()
        {
            try
            {
                NameValueCollection form = HttpContext.Current.Request.Form;
                int pageIndex = ValueTypeParser.Parse(form["page"], 1);
                int pageSize = ValueTypeParser.Parse(form["rp"], 10);
                //string qtype = form["qtype"];
                string query = form["query"];
                //string sortname = form["sortname"];
                //string sortorder = form["sortorder"];
                //string action = form["action"].Trim();

                int totalRowCount;
                int? exceptionId = null;
                string machineName = null;
                string source = null;
                string targetSite = null;
                string exceptionType = null;
                string appDomainName = null;
                string message = null;
                string sessionid = null;
                DateTime? timestampFrom = null;
                DateTime? timestampTo = null;
                string searchText = null;

                if (!string.IsNullOrEmpty(query))
                {
                    NameValueCollection col = ConvertToCollection(query);
                    Array.ForEach(col.AllKeys, o =>
                                                   {
                                                       switch (o.ToLower())
                                                       {
                                                           case "id":
                                                               if (!string.IsNullOrEmpty(col[o]))
                                                               {
                                                                   try
                                                                   {
                                                                       exceptionId = !string.IsNullOrEmpty(col[o])
                                                                                         ? (int?)int.Parse(col[o])
                                                                                         : null;
                                                                   }
                                                                   catch (Exception)
                                                                   {
                                                                       exceptionId = null;
                                                                   }
                                                               }
                                                               break;
                                                           case "machinename":
                                                               if (!string.IsNullOrEmpty(col[o]))
                                                                   machineName = col[o];
                                                               break;
                                                           case "source":
                                                               if (!string.IsNullOrEmpty(col[o]))
                                                                   source = col[o];
                                                               break;
                                                           case "targetsite":
                                                               if (!string.IsNullOrEmpty(col[o]))
                                                                   targetSite = col[o];
                                                               break;
                                                           case "exceptiontype":
                                                               if (!string.IsNullOrEmpty(col[o]))
                                                                   exceptionType = col[o];
                                                               break;
                                                           case "appdomainname":
                                                               if (!string.IsNullOrEmpty(col[o]))
                                                                   appDomainName = col[o];
                                                               break;
                                                           case "message":
                                                               message = col[o];
                                                               break;
                                                           case "timestampf":
                                                               {
                                                                   timestampFrom = ValueTypeParser.Parse(col[o], null, null);
                                                               }
                                                               break;
                                                           case "timestampt":
                                                               {
                                                                   timestampTo = ValueTypeParser.Parse(col[o], null, null);
                                                                   if (timestampTo != null)
                                                                       timestampTo = timestampTo.Value.AddDays(1);
                                                               }
                                                               break;
                                                           case "sessionid":
                                                               {
                                                                   sessionid = col[o];
                                                               }
                                                               break;
                                                           case "searchtext":
                                                               if (!string.IsNullOrEmpty(col[o]))
                                                                   searchText = col[o];
                                                               break;
                                                       }
                                                   });

                    if (timestampFrom == null || timestampTo == null)
                    {
                        timestampFrom = timestampTo = null;
                    }
                    else
                    {
                        int iCnt = DateTime.Compare(timestampFrom.Value, timestampTo.Value);
                        if (iCnt > 0)
                        {
                            DateTime dt = timestampTo.Value;
                            timestampTo = timestampFrom.Value;
                            timestampFrom = dt;
                        }
                    }
                }

                machineName = string.IsNullOrEmpty(machineName) ? null : machineName;
                source = string.IsNullOrEmpty(source) ? null : source;
                targetSite = string.IsNullOrEmpty(targetSite) ? null : targetSite;
                exceptionType = string.IsNullOrEmpty(exceptionType) ? null : exceptionType;
                appDomainName = string.IsNullOrEmpty(appDomainName) ? null : appDomainName;
                message = string.IsNullOrEmpty(message) ? null : message;
                searchText = string.IsNullOrEmpty(searchText) ? null : searchText;
                sessionid = string.IsNullOrEmpty(sessionid) ? null : sessionid;
                var manager = new TripsErrorManager();
                return manager.GetExceptions(exceptionId, machineName, source, targetSite,
                                             exceptionType, appDomainName, message, timestampFrom, timestampTo, pageIndex,
                                             pageSize, searchText, sessionid, out totalRowCount);
            }
            catch (Exception exception)
            {
                return exception.ToString();
            }
        }

        public string GetLogs()
        {
            try
            {
                NameValueCollection form = HttpContext.Current.Request.Form;
                string query = form["query"];
                int? logId = null;
                DateTime? timestampFrom = null;
                DateTime? timestampTo = null;
                string machineName = null;
                string sessionId = null;
                string status = null;
                float? timeMin = null;
                float? timeMax = null;
                string searchText = null;
                int pageIndex = ValueTypeParser.Parse(form["page"], 1);
                int pageSize = ValueTypeParser.Parse(form["rp"], 10);
                string serviceName = null;
                string title = null;
                float timeTaken = 0f;
                int totalRowCount;

                if (!string.IsNullOrEmpty(query))
                {
                    #region comment

                    NameValueCollection col = ConvertToCollection(query);
                    Array.ForEach(col.AllKeys, o =>
                                                   {
                                                       switch (o.ToLower())
                                                       {
                                                           case "id":
                                                               if (!string.IsNullOrEmpty(col[o]))
                                                               {
                                                                   try
                                                                   {
                                                                       logId = !string.IsNullOrEmpty(col[o])
                                                                                   ? (int?)int.Parse(col[o])
                                                                                   : null;
                                                                   }
                                                                   catch (Exception)
                                                                   {
                                                                       logId = null;
                                                                   }
                                                               }
                                                               break;
                                                           case "machinename":
                                                               if (!string.IsNullOrWhiteSpace(col[o]))
                                                                   machineName = col[o];
                                                               break;
                                                           case "sessionid":
                                                               {
                                                                   sessionId = col[o];
                                                               }
                                                               break;
                                                           case "servicename":
                                                               if (!string.IsNullOrEmpty(col[o]))
                                                                   serviceName = col[o];
                                                               break;
                                                           case "title":
                                                               {
                                                                   title = col[o];
                                                               }
                                                               break;
                                                           case "timetaken":
                                                               float.TryParse(col[o], out timeTaken);
                                                               break;
                                                           case "status":
                                                               if (!string.IsNullOrEmpty(col[o]))
                                                                   status = col[o];
                                                               break;
                                                           case "timestampf":
                                                               {
                                                                   timestampFrom = ValueTypeParser.Parse(col[o], null, null);
                                                               }
                                                               break;
                                                           case "timestampt":
                                                               {
                                                                   timestampTo = ValueTypeParser.Parse(col[o], null, null);
                                                               }
                                                               break;
                                                           case "timemax":
                                                               {
                                                                   try
                                                                   {
                                                                       timeMax = ValueTypeParser.Parse<float>(col[o],
                                                                                                              int.MaxValue);
                                                                   }
                                                                   catch (Exception)
                                                                   {
                                                                       timeMax = null;
                                                                   }
                                                               }
                                                               break;
                                                           case "timemin":
                                                               {
                                                                   try
                                                                   {
                                                                       timeMin = ValueTypeParser.Parse<float>(col[o], 0);
                                                                   }
                                                                   catch (Exception)
                                                                   {
                                                                       timeMin = null;
                                                                   }
                                                               }
                                                               break;
                                                           case "searchtext":
                                                               if (!string.IsNullOrEmpty(col[o]))
                                                                   searchText = col[o];
                                                               break;
                                                       }
                                                   });

                    if (timestampFrom == null || timestampTo == null)
                    {
                        timestampFrom = timestampTo = null;
                    }
                    else
                    {
                        int iCnt = DateTime.Compare(timestampFrom.Value, timestampTo.Value);
                        if (iCnt > 0)
                        {
                            DateTime dt = timestampTo.Value;
                            timestampTo = timestampFrom.Value;
                            timestampFrom = dt;
                        }
                    }

                    if (timeMin == null || timeMax == null)
                    {
                        timeMin = timeMax = null;
                    }
                    else
                    {
                        if (timeMin > timeMax)
                        {
                            float? x = timeMin;
                            timeMin = timeMax;
                            timeMax = x;
                        }
                    }

                    #endregion
                }

                return new TripsErrorManager().GetLogs(logId, timestampFrom, timestampTo, machineName, sessionId,
                                                       serviceName, title, timeTaken, status, timeMin, timeMax, searchText,
                                                       pageIndex, pageSize, out totalRowCount);
            }
            catch (Exception exception)
            {
                return exception.ToString();
            }
        }

        #endregion

        private NameValueCollection ConvertToCollection(string str)
        {
            var col = new NameValueCollection();
            string[] args = str.Split('Ω');
            if (args.Length > 0)
            {
                Array.ForEach(args, o =>
                                        {
                                            string[] s = o.Split('§');
                                            if (s.Length == 2)
                                            {
                                                col.Add(s[0].Trim(), s[1].Trim());
                                            }
                                        });
            }
            return col;
        }
    }
}