using System.ServiceModel;
using System.ServiceModel.Web;

namespace TripsErrorUI.Services
{
    [ServiceContract]
    public interface IReportService
    {
        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare,
            UriTemplate = "/Exceptions")]
        string GetExceptions();

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare,
            UriTemplate = "/Logs")]
        string GetLogs();
    }
}