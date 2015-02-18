using System;
using System.IO;
using System.Net;
using TravelWithMe.API.Core.Model.Extensions;

namespace TravelWithMe.API.Core.Utilities
{
    public class RequestResponseBuilder : IDisposable
    {
        public string Error { get; set; }

        public WebRequest Request { get; set; }

        public WebResponse Response { get; set; }

        #region IDisposable Members

        public void Dispose()
        {
            if (Response != null)
                Response.Close();
        }

        #endregion

        public WebRequest CreateRequest<T>(string url, string content, string method = "GET", int timeoutSecs = 360,
                                           string contentType = "application/x-www-form-urlencoded")
            where T : WebRequest
        {
            try
            {
                WebRequest webRequest = WebRequest.Create(url);
                webRequest.Method = method;
                webRequest.Timeout = 1000*timeoutSecs;
                webRequest.ContentType = contentType;
                if (webRequest is HttpWebRequest)
                {
                    //((HttpWebRequest)webRequest).Referer = ;
                    ((HttpWebRequest) webRequest).UserAgent =
                        "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/536.5 (KHTML, like Gecko) Chrome/19.0.1084.52 Safari/536.5";
                }

                if (!string.Equals(method, "GET", StringComparison.OrdinalIgnoreCase))
                {
                    webRequest.ContentLength = content.Length;


                    using (var writer = new StreamWriter(webRequest.GetRequestStream()))
                    {
                        writer.Write(content);
                    }
                }
                Request = webRequest;
                return webRequest as T;
            }
            catch (Exception exception)
            {
                Error = exception.Message;
            }
            return null;
        }

        public WebResponse GetResponse(WebRequest webRequest, out string responseString)
        {
            WebResponse response = null;
            responseString = string.Empty;
            if (webRequest == null)
                return response;
            try
            {
                response = webRequest.GetResponse();
                // Return the response
                if (response != null)
                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                        responseString = reader.ReadToEnd();
                    }
                return response;
            }
            catch (WebException wex)
            {
                responseString = wex.ExtractResponse();
                response = wex.Response;
                Error = wex.Message;
                //wex.Data.Add("Response", wex.ExtractResponse());
                //throw new Exception(wex.Message, wex);
            }
            catch (Exception exception)
            {
                Error = exception.Message;
            }

            Response = response;
            return response;
        }
    }
}