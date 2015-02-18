using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace TravelWithMe.Logging.Helper
{
    public class ExceptionLog
    {
        public ExceptionLog()
        {
            TimeStamp = DateTime.Now;
            MachineName = Environment.MachineName;
            AppDomainName = AppDomain.CurrentDomain.FriendlyName;
        }

        public ExceptionLog(Exception exception, string source, string method, Severity severity, string title = "")
            : this()
        {
            Type = exception.GetType().Name;
            StackTrace = exception.StackTrace;
            Source = string.IsNullOrEmpty(source) ? exception.Source : source;
            Severity = severity.ToString();
            Message = exception.Message;
            InnerException = exception.InnerException != null ? exception.InnerException.ToString() : string.Empty;
            AdditionalInfo = FormatExceptionData(exception.Data);
            TargetSite = exception.TargetSite == null ? "" : exception.TargetSite.Name;
            Title = string.IsNullOrEmpty(title) ? method : title;
        }

        public int ExceptionId { get; set; }

        public string AdditionalInfo { get; set; }

        public string AppDomainName { get; set; }

        public string InnerException { get; set; }

        public string MachineName { get; set; }

        public string Message { get; set; }

        public string SessionId { get; set; }

        public string Source { get; set; }

        public string StackTrace { get; set; }

        public string TargetSite { get; set; }

        public DateTime TimeStamp { get; set; }

        public string Title { get; set; }

        public string Type { get; set; }

        public string Severity { get; set; }

        private static string FormatExceptionData(IDictionary iDictionary)
        {
            if (iDictionary != null)
            {
                var info = new StringBuilder();
                foreach (string key in iDictionary.Keys)
                {
                    info.AppendLine(key + " => " + iDictionary[key]);
                }
                return info.ToString();
            }
            return string.Empty;
        }
    }

    public enum Severity
    {
        Critical,
        Major,
        Normal,
        Trace,
    }
}
