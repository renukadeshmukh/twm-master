using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TravelWithMe.API.Services.DataContract
{
    public class CreateSessionResponse : BaseResponse
    {
        public string SessionId { get; set; }
    }

    public class GetSessionResponse : BaseResponse
    {
        public SessionData Session { get; set; }
    }

    public class SaveBookingInfoResponse : BaseResponse
    {
        
    }

    public class BookingInfoResponse : BaseResponse
    {
        public BookingInformation BookingInfo { get; set; }
        public List<BookedSeat> BookedSeats { get; set; }
    }
}
