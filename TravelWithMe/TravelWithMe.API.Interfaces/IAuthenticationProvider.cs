using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TravelWithMe.API.Interfaces
{
    public interface IAuthenticationProvider
    {
        string CreateAuthenticationId(string accountId);

        bool Validate(string authenticationId);

        string GetAccountId(string authenticationId);

        bool Logout(string authenticationId);
    }
}
