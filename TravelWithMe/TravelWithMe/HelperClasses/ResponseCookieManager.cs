using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using TravelWithMe.API.Services.ServiceImplementation;

namespace TravelWithMe.HelperClasses
{
    public class ResponseCookieManager
    {
        private readonly HttpCookieCollection _httpCookies;

        public ResponseCookieManager(HttpCookieCollection httpCookies)
        {
            _httpCookies = httpCookies;
            _httpCookies.Add(new HttpCookie(AppConfig.CookieName));
        }

        public void SetSessionData(Session session)
        {
            if (session != null)
            {
                var serializer = new JavaScriptSerializer();
                string jsonSession = serializer.Serialize(session);
                _httpCookies.Set(new HttpCookie(AppConfig.CookieName, jsonSession));
            }
        }
    }
}