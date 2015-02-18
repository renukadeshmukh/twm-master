using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TravelWithMe.API.Services.DataContract
{
    public class SessionData
    {
        public string SessionId { get; set; }

        public Account UserAccount { get; set; }

        public BookingInformation BookingInfo { get; set; }
    }
}
