using System;
using TravelWithMe.Data.MySql.Driver;
using System.Collections.Generic;
using TravelWithMe.API.Core.Model;
using System.Linq;
using System.Text;
using TravelWithMe.Data.Interfaces;
using MySql.Data.MySqlClient;
using System.Data;
using TravelWithMe.Data.MySql.Entities;
using TravelWithMe.Logging.Helper;

namespace TravelWithMe.Data.MySql
{
    public class AccountDataProvider : IAccountDataProvider
    {
        private string Source = "AccountDataProvider";

        #region IAccountDataProvider Members

        public Account GetAccount(string email, string password)
        {
            try
            {
                var db = new MySqlDatabase(DbConfiguration.DatabaseRead);
                MySqlCommand command = CommandBuilder.BuildGetAccountCommand(email, password, db.Connection);
                DataSet dataSet = db.ExecuteQuery(command);
                Account account = GetAccount(dataSet);
                return account;
            }
            catch (Exception ex)
            {
                DBExceptionLogger.LogException(ex, Source, "GetAccount", Severity.Critical);
                return null;
            }
        }

        public Account UpdatePersonalInfo(Account account, out string errorMessage)
        {
            errorMessage = string.Empty;
            try
            {
                int addressId = 0;
                var db = new MySqlDatabase(DbConfiguration.DatabaseWrite);
                MySqlCommand cmdUpdatePersonalInfo = CommandBuilder.BuildspUpdatePersonalInfoCommand(db.Connection,
                                                                                                     account.AccountId,
                                                                                                     account.FirstName,
                                                                                                     account.LastName,
                                                                                                     addressId);
                db.ExecuteNonQuery(cmdUpdatePersonalInfo);
                return account;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                DBExceptionLogger.LogException(ex, Source, "UpdateAccount", Severity.Critical);
                return null;
            }
        }

        public Account CreateAccount(Account account, string hashPwd)
        {
            try
            {
                int accountId;
                var db = new MySqlDatabase(DbConfiguration.DatabaseWrite);
                MySqlCommand cmd = CommandBuilder.BuildCreateAccountCommand(db.Connection, account, hashPwd);
                db.ExecuteNonQuery(cmd, "outaccountid", out accountId);
                if (accountId != 0)
                {
                    account.AccountId = accountId.ToString();
                    return account;
                }
            }
            catch (Exception ex)
            {
                DBExceptionLogger.LogException(ex, Source, "CreateAccount", Severity.Critical);
                return null;
            }
            return null;
        }

        public void CreateSocialAccount(string accountId, string socialAccountId, string socialAccountType)
        {
            try
            {
                var db = new MySqlDatabase(DbConfiguration.DatabaseWrite);
                MySqlCommand cmd = CommandBuilder.BuildCreateSocialAccountCommand(db.Connection, accountId,
                                                                                  socialAccountId, socialAccountType);
                db.ExecuteNonQuery(cmd);
            }
            catch (Exception ex)
            {
                DBExceptionLogger.LogException(ex, Source, "CreateSocialAccount", Severity.Critical);
            }
        }

        public bool UpdatePassword(string accountId, string hashCurrPwd, string hashNewPwd)
        {
            try
            {
                var db = new MySqlDatabase(DbConfiguration.DatabaseWrite);
                int result;
                db.ExecuteNonQuery(
                    CommandBuilder.BuildUpdatePasswordCommand(accountId, hashCurrPwd, hashNewPwd, db.Connection),
                    "result", out result);
                return result == 1;
            }
            catch (Exception ex)
            {
                DBExceptionLogger.LogException(ex, Source, "UpdatePassword", Severity.Critical);
                return false;
            }
        }

        public string GetAccountIdByEmail(string email)
        {
            try
            {
                var db = new MySqlDatabase(DbConfiguration.DatabaseRead);

                DataSet dataSet = db.ExecuteQuery(CommandBuilder.BuildGetAccountIdCommand(email, db.Connection));

                return GetAccountId(dataSet);
            }
            catch (Exception ex)
            {
                DBExceptionLogger.LogException(ex, Source, "GetAccountIdByEmail", Severity.Critical);
                return string.Empty;
            }
        }

        public string GetAccountIdForSocialAccount(string accountId, string accountType)
        {
            try
            {
                var db = new MySqlDatabase(DbConfiguration.DatabaseRead);

                DataSet dataSet =
                    db.ExecuteQuery(CommandBuilder.GetAccountIdForSocialAccountCommand(accountId, accountType,
                                                                                       db.Connection));

                return GetAccountId(dataSet);
            }
            catch (Exception ex)
            {
                DBExceptionLogger.LogException(ex, Source, "GetAccountIdForSocialAccount", Severity.Critical);
                return string.Empty;
            }
        }

        public string GetAccountIdForMobile(string mobile)
        {
            try
            {
                var db = new MySqlDatabase(DbConfiguration.DatabaseRead);

                DataSet dataSet =
                    db.ExecuteQuery(CommandBuilder.BuildGetAccountIdForMobileCommand(mobile, db.Connection));

                return GetAccountId(dataSet);
            }
            catch (Exception ex)
            {
                DBExceptionLogger.LogException(ex, Source, "GetAccountIdForMobile", Severity.Critical);
                return string.Empty;
            }
        }

        public bool Exists(string accountId)
        {
            try
            {
                var db = new MySqlDatabase(DbConfiguration.DatabaseRead);

                DataSet dataSet = db.ExecuteQuery(CommandBuilder.BuildAccountExistsCommand(accountId, db.Connection));

                if (dataSet != null && dataSet.Tables.Count > 0)
                {
                    if (dataSet.Tables[0].Rows != null && dataSet.Tables[0].Rows.Count == 1)
                    {
                        DataRow row = dataSet.Tables[0].Rows[0];
                        if (Convert.IsDBNull(row[0]))
                            return false;
                        return row[0].GetInt() == 1;
                    }
                }
            }
            catch (Exception ex)
            {
                DBExceptionLogger.LogException(ex, Source, "Exists(string)", Severity.Critical);
                return false;
            }
            return false;
        }

        public bool Exists(string socialAccountId, string socialAccountType)
        {
            try
            {
                var db = new MySqlDatabase(DbConfiguration.DatabaseRead);

                DataSet dataSet =
                    db.ExecuteQuery(CommandBuilder.BuildSocialAccountExistsCommand(socialAccountId, socialAccountType,
                                                                                   db.Connection));

                if (dataSet != null && dataSet.Tables.Count > 0)
                {
                    if (dataSet.Tables[0].Rows != null && dataSet.Tables[0].Rows.Count == 1)
                    {
                        DataRow row = dataSet.Tables[0].Rows[0];
                        if (Convert.IsDBNull(row[0]))
                            return false;
                        return row[0].GetInt() == 1;
                    }
                }
            }
            catch (Exception ex)
            {
                DBExceptionLogger.LogException(ex, Source, "Exists(string,string)", Severity.Critical);
                return false;
            }
            return false;
        }

        public Account GetAccount(string accountId)
        {
            try
            {
                var db = new MySqlDatabase(DbConfiguration.DatabaseRead);
                DataSet dataSet = db.ExecuteQuery(CommandBuilder.BuildGetAccountByIdCommand(accountId, db.Connection));

                Account account = GetAccount(dataSet);
                return account;
            }
            catch (Exception ex)
            {
                DBExceptionLogger.LogException(ex, Source, "GetAccount(accountId)", Severity.Critical);
                return null;
            }
        }

        public void SetEmailActivated(string email)
        {
            try
            {
                var db = new MySqlDatabase(DbConfiguration.DatabaseWrite);
                MySqlCommand cmd = CommandBuilder.BuildSetEmailActivatedCommand(db.Connection, email);
                db.ExecuteNonQuery(cmd);
            }
            catch (Exception ex)
            {
                DBExceptionLogger.LogException(ex, Source, "SetEmailActivated", Severity.Critical);
            }
        }

        public void SetMobileVerified(string email)
        {
            try
            {
                var db = new MySqlDatabase(DbConfiguration.DatabaseWrite);
                MySqlCommand cmd = CommandBuilder.BuildSetMobileVerifiedCommand(db.Connection, email);
                db.ExecuteNonQuery(cmd);
            }
            catch (Exception ex)
            {
                DBExceptionLogger.LogException(ex, Source, "SetMobileVerified", Severity.Critical);
            }
        }

        public bool SetPassword(string accountId, string hashPwd)
        {
            try
            {
                var db = new MySqlDatabase(DbConfiguration.DatabaseWrite);
                db.ExecuteNonQuery(CommandBuilder.BuildSetPasswordCommand(accountId, hashPwd, db.Connection));
                return true;
            }
            catch (Exception ex)
            {
                DBExceptionLogger.LogException(ex, Source, "SetPassword", Severity.Critical);
            }
            return false;
        }

        public bool ChangeMobile(string accountId, string mobile)
        {
            try
            {
                var db = new MySqlDatabase(DbConfiguration.DatabaseWrite);
                int result;
                db.ExecuteNonQuery(CommandBuilder.BuildChangeMobileCommand(accountId, mobile, db.Connection), "result",
                                   out result);
                return result == 1;
            }
            catch (Exception ex)
            {
                DBExceptionLogger.LogException(ex, Source, "ChangeMobile", Severity.Critical);
                return false;
            }
        }

        public bool ChangeEmail(string accountId, string email)
        {
            try
            {
                var db = new MySqlDatabase(DbConfiguration.DatabaseWrite);
                int result;
                db.ExecuteNonQuery(CommandBuilder.BuildChangeEmailCommand(accountId, email, db.Connection), "result",
                                   out result);
                return result == 1;
            }
            catch (Exception ex)
            {
                DBExceptionLogger.LogException(ex, Source, "ChangeEmail", Severity.Critical);
                return false;
            }
        }

        #endregion

        public bool DeleteAccount(string accountId)
        {
            try
            {
                var db = new MySqlDatabase(DbConfiguration.DatabaseWrite);
                MySqlCommand cmd = CommandBuilder.BuildDeleteAccountCommand(db.Connection, accountId);
                db.ExecuteNonQuery(cmd);
                return true;
            }
            catch (Exception ex)
            {
                DBExceptionLogger.LogException(ex, Source, "DeleteAccount", Severity.Critical);
                return false;
            }
        }

        private static Account GetAccount(DataSet dataSet)
        {
            if (dataSet != null && dataSet.Tables.Count > 0)
            {
                if (dataSet.Tables[0].Rows != null && dataSet.Tables[0].Rows.Count == 1)
                {
                    DataRow row = dataSet.Tables[0].Rows[0];
                    if (Convert.IsDBNull(row["AccountId"]))
                        return null;
                    var account = new Account
                    {
                        AccountId = row["AccountId"].GetString(),
                        Email = row["Email"].GetString(),
                        PhoneNumber = row["Mobile"].GetString(),
                        FirstName = row["FirstName"].GetString(),
                        LastName = row["LastName"].GetString(),
                        AccountType = (AccountType)Enum.Parse(typeof(AccountType), row["AccountType"].GetString()),
                        IsEnabled = Convert.ToBoolean(row["IsEnabled"]),
                    };
                    return account;
                }
            }
            return null;
        }

        private static Address GetAddress(int addressId)
        {
            var db = new MySqlDatabase(DbConfiguration.DatabaseRead);

            DataSet dataSet = db.ExecuteQuery(CommandBuilder.BuildGetAddressCommand(addressId, db.Connection));

            if (dataSet != null && dataSet.Tables.Count > 0)
            {
                if (dataSet.Tables[0].Rows != null && dataSet.Tables[0].Rows.Count == 1)
                {
                    DataRow row = dataSet.Tables[0].Rows[0];
                    if (Convert.IsDBNull(row["AddressId"]))
                        return null;
                    var address = new Address
                    {
                        Id = addressId,
                        AddressLine1 =
                            row["AddressLine1"].GetString(),
                        AddressLine2 =
                            row["AddressLine2"].GetString(),
                        City =
                            row["CityName"].GetString(),
                        Country =
                            row["CountryCode"].GetString(),
                        State = row["State"].GetString(),
                        ZipCode =
                            row["ZipCode"].GetString(),
                    };
                    return address;
                }
            }
            return null;
        }

        private static string GetAccountId(DataSet dataSet)
        {
            if (dataSet != null && dataSet.Tables.Count > 0)
            {
                if (dataSet.Tables[0].Rows != null && dataSet.Tables[0].Rows.Count == 1)
                {
                    DataRow row = dataSet.Tables[0].Rows[0];
                    if (Convert.IsDBNull(row["AccountId"]))
                        return null;
                    return row["AccountId"].GetString();
                }
            }
            return null;
        }

        public bool SaveOrUpdateBusOperator(BusOperator busOperator, out string errorMessage)
        {
            errorMessage = string.Empty;
            var db = new MySqlDatabase(DbConfiguration.DatabaseWrite);
            MySqlCommand cmd = null;
            try
            {
                cmd = AccountCommandBuilder.BuildSaveBusOperatorCommand(db.Connection, busOperator);
                db.ExecuteNonQuery(cmd);
                return true;
            }
            catch (Exception ex)
            {
                errorMessage = "Failed to save Bus operator information";
                DBExceptionLogger.LogException(ex, Source, "SaveOrUpdateBusOperator", Severity.Critical);
                return false;
            }
        }

        public bool SaveOrUpdateBankAccount(BankAccount bankAccount, out int bankAccountId, out string errorMessage)
        {
            errorMessage = string.Empty;
            bankAccountId = 0;
            var db = new MySqlDatabase(DbConfiguration.DatabaseWrite);
            MySqlCommand cmd = null;
            try
            {
                cmd = AccountCommandBuilder.BuildSaveBankAccountCommand(db.Connection, bankAccount);
                db.ExecuteNonQuery(cmd, "outId", out bankAccountId);
                return true;
            }
            catch (Exception ex)
            {
                errorMessage = "Failed to save user bank account information";
                DBExceptionLogger.LogException(ex, Source, "SaveOrUpdateBankAccount", Severity.Critical);
                return false;
            }
        }

        public bool SaveOrUpdateAddress(int accountId, Address address, out int addressId, out string errorMessage)
        {
            errorMessage = string.Empty;
            addressId = 0;
            var db = new MySqlDatabase(DbConfiguration.DatabaseWrite);
            MySqlCommand cmd = null;
            try
            {
                cmd = AccountCommandBuilder.BuildSaveUserAddressCommand(db.Connection, accountId, address);
                db.ExecuteNonQuery(cmd, "outId", out addressId);
                return true;
            }
            catch (Exception ex)
            {
                errorMessage = "Failed to save user address information";
                DBExceptionLogger.LogException(ex, Source, "SaveOrUpdateAddress", Severity.Critical);
                return false;
            }
        }

        public BusOperator GetBusOperatorInfo(int busOperatorId)
        {
            try
            {
                var db = new MySqlDatabase(DbConfiguration.DatabaseRead);
                DataSet dataSet = db.ExecuteQuery(AccountCommandBuilder.BuildGetOperatorInfoByIdCommand(db.Connection, busOperatorId));
                BusOperator busOperator = GetBusOperator(dataSet);
                return busOperator;
            }
            catch (Exception ex)
            {
                DBExceptionLogger.LogException(ex, Source, "GetBusOperatorInfo(busOperatorId)", Severity.Critical);
                return null;
            }
        }

        private BusOperator GetBusOperator(DataSet dataSet)
        {
            if (dataSet != null && dataSet.Tables.Count > 0)
            {
                if (dataSet.Tables[0].Rows != null && dataSet.Tables[0].Rows.Count == 1)
                {
                    DataRow row = dataSet.Tables[0].Rows[0];
                    if (Convert.IsDBNull(row["OperatorId"]))
                        return null;
                    var busOperator = new BusOperator()
                    {
                        OperatorId = row["OperatorId"].GetInt(),
                        Email = GetEmails(row["Email"].GetString()),
                        PhoneNumber = GetPhoneNumbers(row["PhoneNumber"].GetString()),
                        ComapanyName = row["AgencyName"].GetString(),
                    };
                    busOperator.Addresses = GetAddresses(busOperator.OperatorId);
                    busOperator.BankAccount = GetBankAccount(busOperator.OperatorId);
                    return busOperator;
                }
            }
            return null;
        }

        private BankAccount GetBankAccount(int operatorId)
        {
            try
            {
                var db = new MySqlDatabase(DbConfiguration.DatabaseRead);
                DataSet dataSet =
                    db.ExecuteQuery(AccountCommandBuilder.BuildUserBankAccountCommand(db.Connection, operatorId));
                if (dataSet != null && dataSet.Tables.Count > 0)
                {
                    if (dataSet.Tables[0].Rows != null && dataSet.Tables[0].Rows.Count == 1)
                    {
                        DataRow row = dataSet.Tables[0].Rows[0];
                        if (Convert.IsDBNull(row["Id"]))
                            return null;
                        var bankAccount = new BankAccount()
                                              {
                                                  Id = row["Id"].GetInt(),
                                                  UserId = row["UserId"].GetInt(),
                                                  BankName = row["BankName"].GetString(),
                                                  IFSCCode = row["IFSCCode"].GetString(),
                                                  Branch = row["Branch"].GetString(),
                                                  AccountHolderName = row["AccountHolderName"].GetString(),
                                                  AccountNumber = row["AccountNumber"].GetString()
                                              };
                        return bankAccount;
                    }
                }
            }
            catch (Exception ex)
            {
                DBExceptionLogger.LogException(ex, Source, "GetBankAccount", Severity.Critical);
                return null;
            }
            return null;
        }

        private List<Address> GetAddresses(int operatorId)
        {
            List<Address> addresses = new List<Address>();
            try
            {
                var db = new MySqlDatabase(DbConfiguration.DatabaseRead);
                DataSet dataSet = db.ExecuteQuery(AccountCommandBuilder.BuildUserAddressesCommand(db.Connection, operatorId));
                if (dataSet != null && dataSet.Tables.Count > 0)
                {
                    if (dataSet.Tables[0].Rows != null && dataSet.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow row in dataSet.Tables[0].Rows)
                        {
                            try
                            {
                                if (Convert.IsDBNull(row["Id"]))
                                    continue;
                                var address = new Address()
                                                  {
                                                      Id = row["Id"].GetInt(),
                                                      AddressType = row["AddressType"].GetString(),
                                                      AddressLine1 = row["AddressLine1"].GetString(),
                                                      AddressLine2 = row["AddressLine2"].GetString(),
                                                      City = row["City"].GetString(),
                                                      State = row["State"].GetString(),
                                                      ZipCode = row["ZipCode"].GetString(),
                                                      Country = row["Country"].GetString(),
                                                  };
                                addresses.Add(address);
                            }
                            catch (Exception ex)
                            {
                                DBExceptionLogger.LogException(ex, Source, "GetAddress", Severity.Critical);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                DBExceptionLogger.LogException(ex, Source, "GetAddresses", Severity.Critical);
                return null;
            }
            return addresses;
        }

        private List<string> GetPhoneNumbers(string phoneNumberString)
        {
            if (string.IsNullOrEmpty(phoneNumberString))
                return null;
            return phoneNumberString.Split('|').ToList();
        }

        private List<string> GetEmails(string emailString)
        {
            if (string.IsNullOrEmpty(emailString))
                return null;
            return emailString.Split('|').ToList();
        }
    }
}
