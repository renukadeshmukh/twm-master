using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Web;

namespace TravelWithMe.API.Core.Infra
{
    public class ApplicationContext
    {
        [ThreadStatic]
        private static ApplicationContext _current;

        public ApplicationContext()
        {
            Items = new ThreadSafeDictionary();
        }

        public ThreadSafeDictionary Items { get; set; }

        public static ApplicationContext Current
        {
            get { return _current; }
            internal set { _current = value; }
        }

        public static string GetSessionId()
        {
            if (Current != null && Current.Items != null)
            {
                string sessionId = Current.Items["SessionId"];
                if (!string.IsNullOrEmpty(sessionId))
                    return Current.Items["SessionId"];
            }
            else
            {
                if (WebOperationContext.Current != null && WebOperationContext.Current.IncomingRequest != null
                    && WebOperationContext.Current.IncomingRequest.UriTemplateMatch != null &&
                    WebOperationContext.Current.IncomingRequest.UriTemplateMatch.QueryParameters != null)
                    return WebOperationContext.Current.IncomingRequest.UriTemplateMatch.QueryParameters["session"];
            }
            return string.Empty;
        }

        public static void SetSessionId(string sessionId)
        {
            if (Current != null && Current.Items != null)
            {
                Current.Items["SessionId"] = sessionId;
            }
        }
    }
}
