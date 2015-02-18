using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using TravelWithMe.API.Services.DataContract;
using System.ServiceModel.Web;

namespace TravelWithMe.API.Services.ServiceContract
{
    [ServiceContract(Name = "IBookingServiceRest", Namespace = "http://www.busswitch.in/Services/2012/08")]
    public interface IBookingService
    {
        [OperationContract]
        [WebGet(UriTemplate = "/booking/getall/{authId}",
            BodyStyle = WebMessageBodyStyle.Bare)]
        List<BookingInformation> GetAllBookings(string authId);

    }
}
