using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Caching;
using TravelWithMe.API.Core.Caching.Http;
using TravelWithMe.API.Core.Model;
using TravelWithMe.API.Interfaces;
using TravelWithMe.API.Logging;
using TravelWithMe.Logging.Helper;

namespace TravelWithMe.API.SessionMgmt
{
    public class SessionProvider : ISessionProvider
    {
        private string Source = "SessionProvider";

        public string CreateSession(string sessionId)
        {
            string key = string.IsNullOrEmpty(sessionId) ? Guid.NewGuid().ToString() : sessionId;
            try
            {
                var existingSession = (SessionData)HttpRuntime.Cache[key];
                if (existingSession == null)
                {
                    SessionData session = new SessionData() { SessionId = key };
                    ApplicationCache.Insert(key, session, null, Cache.NoAbsoluteExpiration,
                                             DateTime.Now.AddHours(1) - DateTime.Now,
                                             CacheItemPriority.Normal,
                                             null);
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, Source, "CreateSession", Severity.Critical);
            }
            return key;
        }

        public SessionData GetSession(string sessionId)
        {
            try
            {
                var session = (SessionData)ApplicationCache.GetValue(sessionId);
                return session;
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, Source, "GetSession", Severity.Critical);
                return null;
            }
        }

        public bool SaveSession(SessionData sessionData)
        {
            bool status = false;
            if (sessionData != null)
            {
                var session = (SessionData)ApplicationCache.GetValue(sessionData.SessionId);
                if (session != null)
                {
                    try
                    {
                        ApplicationCache.SetValue(sessionData.SessionId, sessionData);
                        status = true;
                    }
                    catch (Exception ex)
                    {
                        Logger.LogException(ex, Source, "SaveSession", Severity.Critical);
                        status = false;
                    }
                }
            }
            return status;
        }


        public bool IsBusOperator(string sessionId, string accountId)
        {
            SessionData data = GetSession(sessionId);
            if (data == null || data.UserAccount == null || data.UserAccount.AccountType != AccountType.BusOperator)
                return false;
            if (string.Equals(data.UserAccount.AccountId, accountId))
                return true;
            return false;
        }

        public bool IsAdministrator(string sessionId, string accountId)
        {
            SessionData data = GetSession(sessionId);
            if (data == null || data.UserAccount == null || data.UserAccount.AccountType != AccountType.Administrator)
                return false;
            if (string.Equals(data.UserAccount.AccountId, accountId))
                return true;
            return false;
        }

    }
}
