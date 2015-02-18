using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TravelWithMe.API.Interfaces
{
    public interface ICaptchaProvider
    {
        bool Validate(string challenge, string ipAddress, string response);
    }
}
