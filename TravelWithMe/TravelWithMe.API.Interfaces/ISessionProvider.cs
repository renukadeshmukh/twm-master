using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TravelWithMe.API.Core.Model;

namespace TravelWithMe.API.Interfaces
{
    public interface ISessionProvider
    {
        string CreateSession(string sessionId);

        SessionData GetSession(string sessionId);

        bool SaveSession(SessionData sessionData);

        bool IsBusOperator(string sessionId, string accountId);

        bool IsAdministrator(string sessionId, string accountId);
    }
}
