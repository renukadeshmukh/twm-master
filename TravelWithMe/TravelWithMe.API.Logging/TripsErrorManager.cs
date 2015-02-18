using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TravelWithMe.Data.MySql;
using TravelWithMe.Logging.Helper;

namespace TravelWithMe.API.Logging
{
    public class TripsErrorManager
    {
        public string GetExceptions(int? exceptionId, string machineName, string source, string targetSite,
                                    string exceptionType, string appDomainName, string message, DateTime? timestampFrom,
                                    DateTime? timestampTo, int pageIndex, int pageSize, string searchText,
                                    string sessionId, out int totalRowCount)
        {
            pageIndex = pageIndex < 0 ? 1 : pageIndex;
            pageSize = pageSize < 0 ? 10 : pageSize;

            List<ExceptionLog> lst = new LoggingDataProvider().GetExceptions(exceptionId, machineName, source,
                                                                             targetSite,
                                                                             exceptionType,
                                                                             appDomainName, message, timestampFrom,
                                                                             timestampTo,
                                                                             pageIndex,
                                                                             pageSize, searchText, sessionId,
                                                                             out totalRowCount);
            var flexigridObject = new FlexigridObject
            {
                page = pageIndex,
                total = totalRowCount,
                cellNames = new List<string>
                                                          {
                                                              "Id",
                                                              "SessionId",
                                                              "Title",
                                                              "Severity",
                                                              "Timestamp",
                                                              "MachineName",
                                                              "ExceptionType",
                                                              "Message",
                                                              "Source",
                                                              "AppDomainName",
                                                              "TargetSite",
                                                              "StackTrace",
                                                              "AdditionalInfo",
                                                              "InnerExceptions"
                                                          }
            };

            foreach (ExceptionLog x in lst)
            {
                var cell = new FlexigridRow
                {
                    id = x.ExceptionId.ToString(),
                    cell = new List<string>
                                              {
                                                  x.ExceptionId.ToString(),
                                                  x.SessionId,
                                                  x.Title,
                                                  x.Severity,
                                                  x.TimeStamp.ToString(),
                                                  x.MachineName,
                                                  x.Type,
                                                  x.Message,
                                                  x.Source,
                                                  x.AppDomainName,
                                                  x.TargetSite,
                                                  x.StackTrace,
                                                  x.AdditionalInfo,
                                                  x.InnerException
                                              },
                };

                flexigridObject.rows.Add(cell);
            }

            return Serializer.JSON.Serialize(flexigridObject);
        }

        public string GetLogs(int? id, DateTime? timestampFrom, DateTime? timestampTo, string machineName,
                              string sessionId, string serviceName, string title, float timeTaken, string status,
                              float? timeMin, float? timeMax, string searchText, int pageIndex, int pageSize,
                              out int totalRowCount)
        {
            pageIndex = pageIndex < 0 ? 1 : pageIndex;
            pageSize = pageSize < 0 ? 10 : pageSize;
            List<Log> lst = new LoggingDataProvider().GetLogs(id, timestampFrom, timestampTo, machineName, sessionId,
                                                              serviceName, title, timeTaken, status, timeMin, timeMax,
                                                              searchText, pageIndex, pageSize, out totalRowCount);

            var flexigridObject = new FlexigridObject
            {
                page = pageIndex,
                total = totalRowCount,
                cellNames = new List<string>
                                                          {
                                                              "LogID",
                                                              "SessionId",
                                                              "Timestamp",
                                                              "MachineName",
                                                              "ServiceName",
                                                              "Title",
                                                              "Status",
                                                              "TimeTaken",
                                                              "Request",
                                                              "Response"
                                                          }
            };
            foreach (Log x in lst)
            {
                var cell = new FlexigridRow
                {
                    id = x.LogId.ToString(),
                    cell = new List<string>
                                              {
                                                  x.LogId.ToString(),
                                                  x.SessionId,
                                                  x.TimeStamp.ToString(),
                                                  x.MachineName,
                                                  x.ServiceName,
                                                  x.Name,
                                                  x.Status.ToString(),
                                                  x.TimeTaken.ToString(),
                                                  "Click Here",
                                                  "Click Here"
                                              }
                };

                flexigridObject.rows.Add(cell);
            }

            return Serializer.JSON.Serialize(flexigridObject);
        }

        public string GetRequestXml(int id)
        {
            return new LoggingDataProvider().GetRequestByLogId(id);
        }

        public string GetResponseXml(int id)
        {
            return new LoggingDataProvider().GetResponseByLogId(id);
        }
    }
}
