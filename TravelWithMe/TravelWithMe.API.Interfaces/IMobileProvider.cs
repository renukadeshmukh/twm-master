using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TravelWithMe.API.Core.Model;

namespace TravelWithMe.API.Interfaces
{
    public interface IMobileProvider
    {
        bool VerifyMobile(Account account, string mobileCode);

        void ResendMobileCode(Account account);
    }
}
