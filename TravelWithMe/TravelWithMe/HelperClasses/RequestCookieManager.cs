using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using TravelWithMe.API.Logging;
using TravelWithMe.API.Services.ServiceImplementation;
using TravelWithMe.Logging.Helper;

namespace TravelWithMe.HelperClasses
{
    public class RequestCookieManager
    {
        private readonly HttpCookie _httpCookie;

        private readonly Session _session;

        public RequestCookieManager(HttpCookieCollection httpCookies)
        {
            try
            {
                _httpCookie = httpCookies[AppConfig.CookieName];

                var serializer = new JavaScriptSerializer();
                if (_httpCookie != null && !string.IsNullOrEmpty(_httpCookie.Value))
                    _session = (Session)serializer.Deserialize(_httpCookie.Value, typeof(Session));
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, "RequestCookieManager", "Constructor", Severity.Normal);
            }
        }

        public string GetSessionId()
        {
            return _session != null ? _session.SessionId : null;
        }

        public string GetAuthenticationId()
        {
            return _session != null ? _session.AuthId : null;
        }

        public string GetFullName()
        {
            return _session != null ? (_session.FirstName + " " + _session.LastName) : null;
        }

        public string GetLastPage()
        {
            return _session != null ? _session.LastPage : null;
        }
    }
}