using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TravelWithMe.API.Core.Model;


namespace TravelWithMe.Data.Interfaces
{
    public interface IAccountDataProvider
    {
        Account GetAccount(string email, string password);

        Account CreateAccount(Account account, string hashPwd);

        bool UpdatePassword(string accountId, string hashCurrPwd, string hashNewPwd);

        string GetAccountIdByEmail(string email);

        string GetAccountIdForMobile(string mobile);

        bool Exists(string accountId);

        bool Exists(string socialAccountId, string socialAccountType);

        Account GetAccount(string accountId);

        void SetEmailActivated(string email);

        void SetMobileVerified(string email);

        bool SetPassword(string accountId, string hashPwd);

        void CreateSocialAccount(string accountId, string socialAccountId, string socialAccountType);

        string GetAccountIdForSocialAccount(string email, string accountType);

        bool ChangeMobile(string accountId, string mobile);

        bool ChangeEmail(string accountId, string email);

        Account UpdatePersonalInfo(Account account, out string errorMessage);

        bool SaveOrUpdateBusOperator(BusOperator busOperator, out string errorMessage);

        bool SaveOrUpdateAddress(int accountId, Address address, out int addressId, out string errorMessage);

        bool SaveOrUpdateBankAccount(BankAccount bankAccount, out int bankAccountId, out string errorMessage);

        BusOperator GetBusOperatorInfo(int busOperatorId);
    }
}
