using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TravelWithMe.API.Core.Model;

namespace TravelWithMe.API.Interfaces
{
    public interface IAccountProvider
    {
        Account Login(string email, string password, out string authId);

        bool Register(Account account, out string authId, out bool accountExists);

        bool ForgotPassword(string accountId, out bool accountExists);

        bool ForgotPasswordByEmail(string email, out string errorMessage);

        bool IsValid(string authenticationId);

        bool IsValid(string accountId, string emailCode);

        string GetAccountIdByEmail(string email, string accountType = null);

        bool Exists(string accountId);

        bool Exists(string socialAccountId, string socialAccountType);

        Account GetAccount(string accountId);

        Account UpdatePersonalInfo(Account account, out string errorMessage);

        void SetEmailActivated(Account account);

        void SetMobileVerified(Account account);

        bool UpdatePassword(string accountId, string currentPassword, string newPassword);

        bool RegisterSocial(string socialAccountId, string socialAccountType, string email, string firstName,
                            string lastName, string mobile, string ipAddress, out string authId, out bool accountExists);

        bool MergeSocial(string socialAccountId, string socialAccountType, string email);

        bool ChangeEmail(string accountId, string email);

        bool ChangeMobile(string accountId, string mobile);

        bool UpdatePassword(string accountId, string newPassword);

        BusOperator GetBusOperator(string accountId);

        bool SaveOrUpdateBusOperator(BusOperator busOperator, out string errorMessage);

        bool SaveOrUpdateAddress(int accountId, Address address, out int addressId, out string errorMessage);

        bool SaveOrUpdateBankAccount(BankAccount bankAccount, out int bankAccountId, out string errorMessage);
    }
}
