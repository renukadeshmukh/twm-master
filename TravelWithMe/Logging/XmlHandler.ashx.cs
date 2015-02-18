using System;
using System.Web;
using System.Xml.Linq;
using TravelWithMe.API.Logging;


namespace Logging
{
    /// <summary>
    /// Summary description for XmlHandler
    /// </summary>
    public class XmlHandler : IHttpHandler
    {
        #region IHttpHandler Members

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/xml";
            int id = ValueTypeParser.Parse(context.Request.QueryString["id"], -1);
            string result = null;
            if (id > -1 && string.Compare(context.Request.QueryString["t"], "request", true) == 0)
            {
                result = new TripsErrorManager().GetRequestXml(id);
            }
            else if (id > -1 && string.Compare(context.Request.QueryString["t"], "response", true) == 0)
            {
                result = new TripsErrorManager().GetResponseXml(id);
            }

            if (string.IsNullOrWhiteSpace(result) == false)
            {
                try
                {
                    XElement.Parse(result);
                }
                catch (Exception)
                {
                    context.Response.ContentType = "text/plain";
                }

                context.Response.Write(result);
                context.Response.Flush();
                context.Response.End();
            }
        }

        public bool IsReusable
        {
            get { return false; }
        }

        #endregion
    }
}