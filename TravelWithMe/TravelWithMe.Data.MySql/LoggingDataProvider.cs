using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TravelWithMe.Data.MySql.Driver;
using System.Data;
using TravelWithMe.Logging.Helper;

namespace TravelWithMe.Data.MySql
{
    public class LoggingDataProvider
    {
        public void SaveLog(Log log)
        {
            try
            {
                var db = new MySqlDatabase(DbConfiguration.LogDB);
                db.ExecuteNonQuery(CommandBuilder.BuildSaveLog(log, db.Connection));
            }
            catch (Exception ex)
            {
                var exLog = new ExceptionLog(ex, "LoggingDataProvider", "SaveLog", Severity.Critical);
                SaveException(exLog);
            }
        }

        public void SaveException(ExceptionLog exception)
        {
            try
            {
                var db = new MySqlDatabase(DbConfiguration.LogDB);
                db.ExecuteNonQuery(CommandBuilder.BuildSaveException(exception, db.Connection));
            }
            catch
            {
            }
            ;
        }

        public List<ExceptionLog> GetExceptions(int? exceptionId, string machineName, string source, string targetSite,
                                                string exceptionType, string appDomainName, string message,
                                                DateTime? timestampFrom, DateTime? timestampTo,
                                                int pageIndex, int pageSize, string searchText, string sessionId,
                                                out int totalRowCount)
        {
            totalRowCount = 0;

            try
            {
                var db = new MySqlDatabase(DbConfiguration.LogDB);
                DataSet dataSet =
                    db.ExecuteQuery(CommandBuilder.BuildGetExceptions(exceptionId, machineName, source, targetSite,
                                                                      exceptionType, appDomainName, message,
                                                                      timestampFrom, timestampTo,
                                                                      pageIndex, pageSize, searchText, sessionId,
                                                                      db.Connection), "outtotalRowCount",
                                    out totalRowCount);

                return ParseExceptionLogs(dataSet);
            }
            catch (Exception ex)
            {
                var exLog = new ExceptionLog(ex, "LoggingDataProvider", "GetLogs", Severity.Normal);
                SaveException(exLog);
            }
            return new List<ExceptionLog>();
        }

        private List<ExceptionLog> ParseExceptionLogs(DataSet dataSet)
        {
            var logList = new List<ExceptionLog>();
            if (dataSet != null && dataSet.Tables.Count > 0)
            {
                if (dataSet.Tables[0].Rows != null && dataSet.Tables[0].Rows.Count > 0)
                {
                    logList.AddRange(from DataRow row in dataSet.Tables[0].Rows
                                     select ParseExceptionLog(row));
                }
            }
            return logList;
        }

        private ExceptionLog ParseExceptionLog(DataRow row)
        {
            return new ExceptionLog
            {
                ExceptionId = Convert.ToInt32(row["exceptionId"]),
                AdditionalInfo =
                    Convert.IsDBNull(row["additionalInfo"]) ? null : row["additionalInfo"].ToString(),
                MachineName = row["machineName"].GetString(),
                Title = row["title"].GetString(),
                AppDomainName = row["appDomainName"].GetString(),
                SessionId = row["sessionId"].ToString(),
                InnerException =
                    Convert.IsDBNull(row["InnerException"]) ? null : row["InnerException"].ToString(),
                Severity = row["severity"].ToString(),
                TimeStamp = Convert.ToDateTime(row["timeStamp"].ToString()),
                Message = row["Message"].ToString(),
                Source = row["source"].ToString(),
                StackTrace = Convert.IsDBNull(row["stacktrace"]) ? null : row["stacktrace"].ToString(),
                TargetSite = row["targetSite"].ToString(),
                Type = row["type"].ToString()
            };
        }

        public List<Log> GetLogs(int? id, DateTime? timestampFrom, DateTime? timestampTo, string machineName,
                                 string sessionId, string serviceName, string title, float timeTaken, string status,
                                 float? timeMin,
                                 float? timeMax, string searchText, int pageIndex, int pageSize, out int totalRowCount)
        {
            totalRowCount = 0;

            try
            {
                var db = new MySqlDatabase(DbConfiguration.LogDB);
                DataSet dataSet =
                    db.ExecuteQuery(CommandBuilder.BuildGetLogs(id, timestampFrom, timestampTo, machineName,
                                                                sessionId, serviceName, title, timeTaken, status,
                                                                timeMin,
                                                                timeMax, searchText, pageIndex, pageSize, db.Connection),
                                    "outtotalRowCount", out totalRowCount);

                return ParseLogs(dataSet);
            }
            catch (Exception ex)
            {
                var exLog = new ExceptionLog(ex, "LoggingDataProvider", "GetLogs", Severity.Normal);
                SaveException(exLog);
            }
            return new List<Log>();
        }

        private List<Log> ParseLogs(DataSet dataSet)
        {
            var logList = new List<Log>();
            if (dataSet != null && dataSet.Tables.Count > 0)
            {
                if (dataSet.Tables[0].Rows != null && dataSet.Tables[0].Rows.Count > 0)
                {
                    logList.AddRange(from DataRow row in dataSet.Tables[0].Rows
                                     select ParseLog(row));
                }
            }
            return logList;
        }

        private Log ParseLog(DataRow row)
        {
            return new Log
            {
                LogId = Convert.ToInt32(row["logId"]),
                MachineName = row["machineName"].GetString(),
                Name = row["name"].GetString(),
                ServiceName = row["serviceName"].GetString(),
                SessionId = row["sessionId"].ToString(),
                Status =
                    (Status)Enum.Parse(typeof(Status), row["status"].ToString()),
                TimeStamp = Convert.ToDateTime(row["timeStamp"].ToString()),
                TimeTaken = Convert.ToSingle(row["timeTaken"])
            };
        }

        public string GetRequestByLogId(int logId)
        {
            try
            {
                var db = new MySqlDatabase(DbConfiguration.LogDB);
                DataSet dataSet = db.ExecuteQuery(CommandBuilder.BuildGetRequestByLogId(logId, db.Connection));

                return dataSet != null && dataSet.Tables.Count == 1 && dataSet.Tables[0].Rows != null &&
                       dataSet.Tables[0].Rows.Count == 1
                           ? dataSet.Tables[0].Rows[0][0].ToString()
                           : "No Request Found";
            }
            catch (Exception ex)
            {
                var exLog = new ExceptionLog(ex, "LoggingDataProvider", "GetRequestByLogId", Severity.Normal);
                SaveException(exLog);
            }

            return "No Request Found";
        }

        public string GetResponseByLogId(int logId)
        {
            try
            {
                var db = new MySqlDatabase(DbConfiguration.LogDB);
                DataSet dataSet = db.ExecuteQuery(CommandBuilder.BuildGetResponseByLogId(logId, db.Connection));

                return dataSet != null && dataSet.Tables.Count == 1 && dataSet.Tables[0].Rows != null &&
                       dataSet.Tables[0].Rows.Count == 1
                           ? dataSet.Tables[0].Rows[0][0].ToString()
                           : "No Response Found";
            }
            catch (Exception ex)
            {
                var exLog = new ExceptionLog(ex, "LoggingDataProvider", "GetResponseByLogId", Severity.Normal);
                SaveException(exLog);
            }

            return "No Response Found";
        }
    }
}
