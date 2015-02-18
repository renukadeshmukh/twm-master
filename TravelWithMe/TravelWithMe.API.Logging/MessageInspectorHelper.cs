using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Runtime.Serialization.Json;
using System.Xml;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TravelWithMe.Logging.Helper;
using System.IO;

namespace TravelWithMe.API.Logging
{
    public class MessageInspectorHelper
    {
        public static Log BuildLogRequest(ref Message request, IClientChannel channel)
        {
            string logName = request.Properties.ContainsKey("HttpOperationName")
                                 ? request.Properties["HttpOperationName"].ToString()
                                 : string.Empty;
            string serviceName = GetServiceName(request.Headers.To);
            var httpReq = (HttpRequestMessageProperty)request.Properties[HttpRequestMessageProperty.Name];
            string disableHeaderLog = httpReq.Headers["X-DisableHeaderLog"];
            serviceName = string.IsNullOrEmpty(serviceName) ? httpReq.Headers["X-ServiceName"] : serviceName;
            logName = string.IsNullOrEmpty(logName) ? httpReq.Headers["X-MethodName"] : logName;
            string errorElement = httpReq.Headers["X-SuccessElement"];
            var rq = new StringBuilder();
            if (string.IsNullOrEmpty(disableHeaderLog))
            {
                rq.AppendLine(string.Format("{0} {1}", httpReq.Method, request.Headers.To));
                foreach (string header in httpReq.Headers.AllKeys)
                {
                    rq.AppendLine(string.Format("{0}: {1}", header, httpReq.Headers[header]));
                }
                rq.AppendLine("------------------------------------------------------------");
            }

            if (!request.IsEmpty)
            {
                rq.AppendLine(MessageToString(ref request));
            }
            var log = new Log(logName, rq.ToString(), serviceName);
            log["SuccessElement"] = errorElement; // Used to set the status of the log after receiving response.
            return log;
        }

        public static void LogRequestResponse(ref Message reply, Log log)
        {
            log.EndTime = DateTime.Now;
            var rs = new StringBuilder();
            var httpResp = (HttpResponseMessageProperty)reply.Properties[HttpResponseMessageProperty.Name];
            if (!reply.IsEmpty)
            {
                rs.AppendLine(MessageToString(ref reply));
            }
            log.Response = rs.ToString();
            AssignLogStatus(httpResp, reply, log);
            Logger.LogMessage(log);
        }

        private static void AssignLogStatus(HttpResponseMessageProperty httpResp, Message reply, Log log)
        {
            string errorElement = log["SuccessElement"];
            if (string.IsNullOrEmpty(errorElement)) return;
            if (!log.Response.Contains(errorElement))
                log.Status = Status.Failure;
        }

        private static string GetServiceName(Uri uri)
        {
            string serviceName = string.Empty;
            if (uri != null && uri.Segments != null)
            {
                foreach (string seg in uri.Segments)
                {
                    if (seg.ToLower().Contains(".svc"))
                        serviceName = seg;
                }
            }
            return serviceName;
        }

        private static WebContentFormat GetMessageContentFormat(Message message)
        {
            WebContentFormat format = WebContentFormat.Default;
            if (message.Properties.ContainsKey(WebBodyFormatMessageProperty.Name))
            {
                WebBodyFormatMessageProperty bodyFormat;
                bodyFormat = (WebBodyFormatMessageProperty)message.Properties[WebBodyFormatMessageProperty.Name];
                format = bodyFormat.Format;
            }

            return format;
        }

        private static string MessageToString(ref Message message)
        {
            WebContentFormat messageFormat = GetMessageContentFormat(message);
            var ms = new MemoryStream();
            XmlDictionaryWriter writer = null;
            switch (messageFormat)
            {
                case WebContentFormat.Default:
                case WebContentFormat.Xml:
                    writer = XmlDictionaryWriter.CreateTextWriter(ms);
                    break;
                case WebContentFormat.Json:
                    writer = JsonReaderWriterFactory.CreateJsonWriter(ms);
                    break;
                case WebContentFormat.Raw:
                    // special case for raw, easier implemented separately
                    return ReadRawBody(ref message);
            }

            message.WriteMessage(writer);
            writer.Flush();
            string messageBody = Encoding.UTF8.GetString(ms.ToArray());

            // Here would be a good place to change the message body, if so desired.

            // now that the message was read, it needs to be recreated.
            ms.Position = 0;

            // if the message body was modified, needs to reencode it, as show below
            // ms = new MemoryStream(Encoding.UTF8.GetBytes(messageBody));

            XmlDictionaryReader reader;
            if (messageFormat == WebContentFormat.Json)
            {
                reader = JsonReaderWriterFactory.CreateJsonReader(ms, XmlDictionaryReaderQuotas.Max);
            }
            else
            {
                reader = XmlDictionaryReader.CreateTextReader(ms, XmlDictionaryReaderQuotas.Max);
            }

            Message newMessage = Message.CreateMessage(reader, int.MaxValue, message.Version);
            newMessage.Properties.CopyProperties(message.Properties);
            message = newMessage;

            return messageBody;
        }

        private static string ReadRawBody(ref Message message)
        {
            XmlDictionaryReader bodyReader = message.GetReaderAtBodyContents();
            bodyReader.ReadStartElement("Binary");
            byte[] bodyBytes = bodyReader.ReadContentAsBase64();
            string messageBody = Encoding.UTF8.GetString(bodyBytes);

            // Now to recreate the message
            var ms = new MemoryStream();
            XmlDictionaryWriter writer = XmlDictionaryWriter.CreateBinaryWriter(ms);
            writer.WriteStartElement("Binary");
            writer.WriteBase64(bodyBytes, 0, bodyBytes.Length);
            writer.WriteEndElement();
            writer.Flush();
            ms.Position = 0;
            XmlDictionaryReader reader = XmlDictionaryReader.CreateBinaryReader(ms, XmlDictionaryReaderQuotas.Max);
            Message newMessage = Message.CreateMessage(reader, int.MaxValue, message.Version);
            newMessage.Properties.CopyProperties(message.Properties);
            message = newMessage;

            return messageBody;
        }
    }
}
