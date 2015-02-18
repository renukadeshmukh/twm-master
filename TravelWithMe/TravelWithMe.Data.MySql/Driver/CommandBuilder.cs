using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using TravelWithMe.API.Core.Model;
using TravelWithMe.Logging.Helper;

namespace TravelWithMe.Data.MySql.Driver
{
    public class CommandBuilder
    {
        #region Account functions
        internal static MySqlCommand BuildGetAccountCommand(string email, string password, MySqlConnection connection)
        {
            var cmd = new MySqlCommand("spGetAccount", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add(new MySqlParameter("inemail", email));
            cmd.Parameters.Add(new MySqlParameter("inpwd", password));
            return cmd;
        }

        internal static MySqlCommand BuildGetSocialAccountCommand(string facebookId, string type,
                                                                  MySqlConnection connection)
        {
            var cmd = new MySqlCommand("spGetAccountBySocialId", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add(new MySqlParameter("insocialaccountid", facebookId));
            cmd.Parameters.Add(new MySqlParameter("insocialaccounttype", type));
            return cmd;
        }

        internal static MySqlCommand BuildGetAddressCommand(int addressId, MySqlConnection connection)
        {
            var cmd = new MySqlCommand("spGetBillingAddress", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add(new MySqlParameter("inaddressid", addressId));

            return cmd;
        }

        internal static MySqlCommand BuildUpdateAddressCommand(MySqlConnection connection, Address address)
        {
            var cmd = new MySqlCommand("spSaveOrUpdateAddress", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add(new MySqlParameter("inAddressid", address.Id));
            cmd.Parameters.Add(new MySqlParameter("inAddressLine1", address.AddressLine1));
            cmd.Parameters.Add(new MySqlParameter("inAddressLine2", address.AddressLine2));
            cmd.Parameters.Add(new MySqlParameter("inCityName", address.City));
            cmd.Parameters.Add(new MySqlParameter("inState", address.State));
            cmd.Parameters.Add(new MySqlParameter("inCountryCode", address.Country));
            cmd.Parameters.Add(new MySqlParameter("inZipCode", address.ZipCode));
            cmd.Parameters.Add(new MySqlParameter("outAddressid", MySqlDbType.Int32));
            cmd.Parameters["outAddressid"].Direction = ParameterDirection.Output;
            return cmd;
        }

        internal static MySqlCommand BuildCreateAccountCommand(MySqlConnection connection, Account account, string hashPwd)
        {
            var cmd = new MySqlCommand("spCreateAccount", connection) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.Add(new MySqlParameter("inEmail", account.Email));
            cmd.Parameters.Add(new MySqlParameter("inFirstName", account.FirstName));
            cmd.Parameters.Add(new MySqlParameter("inLastName", account.LastName));
            cmd.Parameters.Add(new MySqlParameter("inPasswordHash", hashPwd));
            cmd.Parameters.Add(new MySqlParameter("inMobile", account.PhoneNumber));
            cmd.Parameters.Add(new MySqlParameter("inAccountType", account.AccountType));
            cmd.Parameters.Add(new MySqlParameter("inIsEnabled", account.IsEnabled));
            cmd.Parameters.Add(new MySqlParameter("outaccountid", MySqlDbType.Int32));
            cmd.Parameters["outaccountid"].Direction = ParameterDirection.Output;
            return cmd;
        }

        internal static MySqlCommand BuildspUpdatePersonalInfoCommand(MySqlConnection connection, string accountId,
                                                                      string firstName, string lastName, int addressId)
        {
            var cmd = new MySqlCommand("spUpdatePersonalInfo", connection) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.Add(new MySqlParameter("inAccountId", accountId));
            cmd.Parameters.Add(new MySqlParameter("inFirstname", firstName));
            cmd.Parameters.Add(new MySqlParameter("inLastname", lastName));
            if (addressId > 0)
                cmd.Parameters.Add(new MySqlParameter("inAddressId", addressId));
            else
            {
                cmd.Parameters.Add(new MySqlParameter("inAddressId", null));
            }
            return cmd;
        }

        internal static MySqlCommand BuildUpdatePasswordCommand(string accountId, string hashPwd, string hashNewPwd,
                                                                MySqlConnection connection)
        {
            var cmd = new MySqlCommand("spUpdatePassword", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add(new MySqlParameter("inaccountId", accountId));
            cmd.Parameters.Add(new MySqlParameter("incurrpwd", hashPwd));
            cmd.Parameters.Add(new MySqlParameter("innewpwd", hashNewPwd));
            cmd.Parameters.Add(new MySqlParameter("result", MySqlDbType.Int32));
            cmd.Parameters["result"].Direction = ParameterDirection.Output;
            return cmd;
        }

        internal static MySqlCommand BuildGetAccountIdCommand(string email, MySqlConnection connection)
        {
            var cmd = new MySqlCommand("spGetAccountIdByEmail", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add(new MySqlParameter("inemail", email));

            return cmd;
        }

        internal static MySqlCommand GetAccountIdForSocialAccountCommand(string accountId, string accountType,
                                                                         MySqlConnection connection)
        {
            var cmd = new MySqlCommand("spGetAccountIdForSocialAccount", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add(new MySqlParameter("insocialaccountId", accountId));
            cmd.Parameters.Add(new MySqlParameter("insocialaccounttype", accountType));

            return cmd;
        }

        internal static MySqlCommand BuildGetAccountIdForMobileCommand(string mobile, MySqlConnection connection)
        {
            var cmd = new MySqlCommand("spGetAccountIdForMobile", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add(new MySqlParameter("inmobile", mobile));

            return cmd;
        }

        internal static MySqlCommand BuildAccountExistsCommand(string accountId, MySqlConnection connection)
        {
            var cmd = new MySqlCommand("spAccountExists", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add(new MySqlParameter("inaccountid", accountId));

            return cmd;
        }

        internal static MySqlCommand BuildSocialAccountExistsCommand(string socialAccountId, string socialAccountType,
                                                                     MySqlConnection connection)
        {
            var cmd = new MySqlCommand("spSocialAccountExists", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add(new MySqlParameter("insocialaccountid", socialAccountId));
            cmd.Parameters.Add(new MySqlParameter("insocialaccounttype", socialAccountType));
            return cmd;
        }

        internal static MySqlCommand BuildGetAccountByIdCommand(string accountId, MySqlConnection connection)
        {
            var cmd = new MySqlCommand("spGetAccountById", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add(new MySqlParameter("inaccountid", accountId));

            return cmd;
        }

        internal static MySqlCommand BuildDeleteAccountCommand(MySqlConnection connection, string accountId)
        {
            var cmd = new MySqlCommand("spDeleteAccount", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add(new MySqlParameter("inaccountId", accountId));
            return cmd;
        }

        internal static MySqlCommand BuildChangeMobileCommand(string accountId, string mobile,
                                                              MySqlConnection connection)
        {
            var cmd = new MySqlCommand("spChangeMobile", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add(new MySqlParameter("inaccountId", accountId));
            cmd.Parameters.Add(new MySqlParameter("inmobile", mobile));
            cmd.Parameters.Add(new MySqlParameter("result", MySqlDbType.Int32));
            cmd.Parameters["result"].Direction = ParameterDirection.Output;

            return cmd;
        }

        internal static MySqlCommand BuildChangeEmailCommand(string accountId, string mobile, MySqlConnection connection)
        {
            var cmd = new MySqlCommand("spChangeEmail", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add(new MySqlParameter("inaccountId", accountId));
            cmd.Parameters.Add(new MySqlParameter("inemail", mobile));
            cmd.Parameters.Add(new MySqlParameter("result", MySqlDbType.Int32));
            cmd.Parameters["result"].Direction = ParameterDirection.Output;

            return cmd;
        }

        internal static MySqlCommand BuildCreateSocialAccountCommand(MySqlConnection connection, string accountId,
                                                                    string socialAccountId, string socialAccountType)
        {
            var cmd = new MySqlCommand("spCreateSocialAccount", connection) { CommandType = CommandType.StoredProcedure };


            cmd.Parameters.Add(new MySqlParameter("inaccountId", accountId));
            cmd.Parameters.Add(new MySqlParameter("insocialAccountId", socialAccountId));
            cmd.Parameters.Add(new MySqlParameter("insocialAccountType", socialAccountType));
            return cmd;
        }

        internal static MySqlCommand BuildSetEmailActivatedCommand(MySqlConnection connection, string email)
        {
            var cmd = new MySqlCommand("spSetEmailActivated", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add(new MySqlParameter("inemail", email));
            return cmd;
        }

        internal static MySqlCommand BuildSetMobileVerifiedCommand(MySqlConnection connection, string email)
        {
            var cmd = new MySqlCommand("spSetMobileVerified", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add(new MySqlParameter("inemail", email));
            return cmd;
        }

        internal static MySqlCommand BuildGetLogs(int? id, DateTime? timestampFrom, DateTime? timestampTo,
                                                  string machineName,
                                                  string sessionId, string serviceName, string name, float timeTaken,
                                                  string status,
                                                  float? timeMin, float? timeMax, string searchText, int pageIndex,
                                                  int pageSize, MySqlConnection connection)
        {
            var cmd = new MySqlCommand("spGetLogs", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add(new MySqlParameter("inlogid", id));
            cmd.Parameters.Add(new MySqlParameter("intimeStampFrom", timestampFrom));
            cmd.Parameters.Add(new MySqlParameter("intimeStampTo", timestampTo));
            cmd.Parameters.Add(new MySqlParameter("inmachineName",
                                                  string.IsNullOrEmpty(machineName) ? null : machineName));
            cmd.Parameters.Add(new MySqlParameter("insessionId", string.IsNullOrEmpty(sessionId) ? null : sessionId));
            cmd.Parameters.Add(new MySqlParameter("inserviceName",
                                                  string.IsNullOrEmpty(serviceName) ? null : serviceName));
            cmd.Parameters.Add(new MySqlParameter("inname", string.IsNullOrEmpty(name) ? null : name));
            cmd.Parameters.Add(new MySqlParameter("intimeTaken", timeTaken));
            cmd.Parameters.Add(new MySqlParameter("instatus", string.IsNullOrEmpty(status) ? null : status));
            cmd.Parameters.Add(new MySqlParameter("intimeMin", timeMin));
            cmd.Parameters.Add(new MySqlParameter("intimeMax", timeMax));
            cmd.Parameters.Add(new MySqlParameter("insearchText", string.IsNullOrEmpty(searchText) ? null : searchText));
            cmd.Parameters.Add(new MySqlParameter("inpageIndex", pageIndex));
            cmd.Parameters.Add(new MySqlParameter("inpageSize", pageSize));
            cmd.Parameters.Add(new MySqlParameter("outtotalRowCount", MySqlDbType.Int32));

            cmd.Parameters["outtotalRowCount"].Direction = ParameterDirection.Output;

            return cmd;
        }

        internal static MySqlCommand BuildGetRequestByLogId(int logId, MySqlConnection connection)
        {
            var cmd = new MySqlCommand("spGetRequestByLogId", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add(new MySqlParameter("inlogid", logId));
            return cmd;
        }

        internal static MySqlCommand BuildGetResponseByLogId(int logId, MySqlConnection connection)
        {
            var cmd = new MySqlCommand("spGetResponseByLogId", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add(new MySqlParameter("inlogid", logId));
            return cmd;
        }

        internal static MySqlCommand BuildGetExceptions(int? exceptionId, string machineName, string source,
                                                        string targetSite,
                                                        string exceptionType, string appDomainName, string message,
                                                        DateTime? timestampFrom, DateTime? timestampTo,
                                                        int pageIndex, int pageSize, string searchText, string sessionId,
                                                        MySqlConnection connection)
        {
            var cmd = new MySqlCommand("spGetExceptions", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add(new MySqlParameter("inexceptionId", exceptionId));
            cmd.Parameters.Add(new MySqlParameter("intimeStampFrom", timestampFrom));
            cmd.Parameters.Add(new MySqlParameter("intimeStampTo", timestampTo));
            cmd.Parameters.Add(new MySqlParameter("inmachineName",
                                                  string.IsNullOrEmpty(machineName) ? null : machineName));
            cmd.Parameters.Add(new MySqlParameter("insessionId", string.IsNullOrEmpty(sessionId) ? null : sessionId));
            cmd.Parameters.Add(new MySqlParameter("insource", string.IsNullOrEmpty(source) ? null : source));
            cmd.Parameters.Add(new MySqlParameter("intargetSite", string.IsNullOrEmpty(targetSite) ? null : targetSite));
            cmd.Parameters.Add(new MySqlParameter("inexceptionType",
                                                  string.IsNullOrEmpty(exceptionType) ? null : exceptionType));
            cmd.Parameters.Add(new MySqlParameter("inappDomainName",
                                                  string.IsNullOrEmpty(appDomainName) ? null : appDomainName));
            cmd.Parameters.Add(new MySqlParameter("inmessage", string.IsNullOrEmpty(message) ? null : message));
            cmd.Parameters.Add(new MySqlParameter("insearchText", string.IsNullOrEmpty(searchText) ? null : searchText));
            cmd.Parameters.Add(new MySqlParameter("inpageIndex", pageIndex));
            cmd.Parameters.Add(new MySqlParameter("inpageSize", pageSize));
            cmd.Parameters.Add(new MySqlParameter("outtotalRowCount", MySqlDbType.Int32));

            cmd.Parameters["outtotalRowCount"].Direction = ParameterDirection.Output;

            return cmd;
        }

        internal static MySqlCommand BuildSaveLog(Log log, MySqlConnection connection)
        {
            var cmd = new MySqlCommand("spSaveLog", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add(new MySqlParameter("inmachineName", log.MachineName));
            cmd.Parameters.Add(new MySqlParameter("inname", log.Name));
            cmd.Parameters.Add(new MySqlParameter("inrequest", log.Request));
            cmd.Parameters.Add(new MySqlParameter("inresponse", log.Response));
            cmd.Parameters.Add(new MySqlParameter("inserviceId", log.ServiceId));
            cmd.Parameters.Add(new MySqlParameter("inserviceName", log.ServiceName));
            cmd.Parameters.Add(new MySqlParameter("insessionId", log.SessionId));
            cmd.Parameters.Add(new MySqlParameter("instatus", log.Status.ToString()));
            cmd.Parameters.Add(new MySqlParameter("intimeStamp", DateTime.Now));
            cmd.Parameters.Add(new MySqlParameter("intimeTaken", log.TimeTaken));
            return cmd;
        }

        internal static MySqlCommand BuildSaveException(ExceptionLog exception, MySqlConnection connection)
        {
            var cmd = new MySqlCommand("spSaveException", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add(new MySqlParameter("inAdditionalInfo", exception.AdditionalInfo));
            cmd.Parameters.Add(new MySqlParameter("inAppDomainName", exception.AppDomainName));
            cmd.Parameters.Add(new MySqlParameter("inInnerException", exception.InnerException));
            cmd.Parameters.Add(new MySqlParameter("inMachineName", exception.MachineName));
            cmd.Parameters.Add(new MySqlParameter("inMessage", exception.Message));
            cmd.Parameters.Add(new MySqlParameter("inSessionId", exception.SessionId));
            cmd.Parameters.Add(new MySqlParameter("inSource", exception.Source));
            cmd.Parameters.Add(new MySqlParameter("inStackTrace", exception.StackTrace));
            cmd.Parameters.Add(new MySqlParameter("inTargetSite", exception.TargetSite));
            cmd.Parameters.Add(new MySqlParameter("inTimeStamp", DateTime.Now));
            cmd.Parameters.Add(new MySqlParameter("inTitle", exception.Title));
            cmd.Parameters.Add(new MySqlParameter("inType", exception.Type));
            cmd.Parameters.Add(new MySqlParameter("inSeverity", exception.Severity));
            return cmd;
        }

        internal static MySqlCommand BuildSetPasswordCommand(string accountId, string hashPwd,
                                                             MySqlConnection connection)
        {
            var cmd = new MySqlCommand("spResetPassword", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add(new MySqlParameter("inaccountId", accountId));
            cmd.Parameters.Add(new MySqlParameter("innewpwd", hashPwd));
            return cmd;
        }

        internal static MySqlCommand BuildRemoveVerificationCodeCommand(string key, string type,
                                                                        MySqlConnection connection)
        {
            var cmd = new MySqlCommand("spRemoveVerificationCode", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add(new MySqlParameter("inkey", key));
            cmd.Parameters.Add(new MySqlParameter("intype", type));

            return cmd;
        }

        internal static MySqlCommand BuildSaveVerificationCodeCommand(string key, string value, string type,
                                                                      MySqlConnection connection)
        {
            var cmd = new MySqlCommand("spSaveVerificationCode", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add(new MySqlParameter("inkey", key));
            cmd.Parameters.Add(new MySqlParameter("invalue", value));
            cmd.Parameters.Add(new MySqlParameter("intype", type));

            return cmd;
        }

        internal static MySqlCommand BuildVerifyVerificationCodeCommand(string key, string value, string type,
                                                                       MySqlConnection connection)
        {
            var cmd = new MySqlCommand("spVerifyVerificationCode", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.Add(new MySqlParameter("inkey", key));
            cmd.Parameters.Add(new MySqlParameter("invalue", value));
            cmd.Parameters.Add(new MySqlParameter("intype", type));

            return cmd;
        }
        #endregion

       
    }
}
