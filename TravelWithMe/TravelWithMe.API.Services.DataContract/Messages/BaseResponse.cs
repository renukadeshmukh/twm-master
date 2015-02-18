using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TravelWithMe.API.Services.DataContract
{
    public class BaseResponse
    {
        public string ErrorMessage { get; set; }

        public bool IsSuccess { get; set; }
    }
}
