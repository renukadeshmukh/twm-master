using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TravelWithMe.API.Logging;
using TravelWithMe.API.Services.DataContract;
using TravelWithMe.HelperClasses;
using TravelWithMe.Logging.Helper;
using TravelWithMe.API.Services.ServiceImplementation;

namespace TravelWithMe.Master
{
    public partial class Main : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                var clientCookie = new RequestCookieManager(Request.Cookies);
                string oldSessionId = clientCookie.GetSessionId();
                string url = string.Format("{0}/get/SessionService.svc/CreateSession?session={1}", AppConfig.BaseSiteUrl, oldSessionId);
                HttpClient client = new HttpClient(url, ContentType.Xml);
                CreateSessionResponse sessionResponse = client.Get<CreateSessionResponse>();
                if (!string.Equals(sessionResponse.SessionId, oldSessionId)) // if its new session then reset cookie.
                {
                    var session = new Session { SessionId = sessionResponse.SessionId };
                    var cookieManager = new ResponseCookieManager(Response.Cookies);
                    cookieManager.SetSessionData(session);
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, "MasterPage", "Page_Load", Severity.Critical);
            }
        }
    }
}