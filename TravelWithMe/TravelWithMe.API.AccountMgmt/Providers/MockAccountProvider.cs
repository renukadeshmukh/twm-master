using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Caching;
using TravelWithMe.API.Interfaces;
using TravelWithMe.API.Core.Model;

namespace TravelWithMe.API.AccountMgmt.Providers
{
    public class MockAccountProvider : IAccountProvider
    {
        #region IAccountProvider Members

        public string _accountKey = "Accounts";

        private readonly IAuthenticationProvider _authenticationProvider;
        public MockAccountProvider(IAuthenticationProvider authenticationProvider)
        {
            _authenticationProvider = authenticationProvider;
            Dictionary<string, Account> accounts = HttpRuntime.Cache[_accountKey] as Dictionary<string, Account>;
            if (accounts == null)
            {
                accounts = new Dictionary<string, Account>();
                accounts["a@b.c"] = new Account()
                                        {
                                            AccountType = AccountType.BusOperator,
                                            AccountId = "1",
                                            Email = "a@b.c",
                                            FirstName = "Renuka",
                                            LastName = "Deshmukh",
                                            PhoneNumber = "+91 7709161990",
                                            IsEnabled = true,
                                        };
                HttpRuntime.Cache.Insert(_accountKey, accounts, null, Cache.NoAbsoluteExpiration,
                                         DateTime.Now.AddHours(24) - DateTime.Now,
                                         CacheItemPriority.Normal,
                                         null);
            }
        }

        public Account Login(string email, string password, out string authId)
        {
            
            authId = string.Empty;
            Dictionary<string, Account> accounts = HttpRuntime.Cache[_accountKey] as Dictionary<string, Account>;
            if (!accounts.ContainsKey(email))
            {
                return null;
            }
            authId = _authenticationProvider.CreateAuthenticationId(accounts[email].AccountId);
            return accounts[email];
        }

        public bool Register(Account account, out string authId, out bool accountExists)
        {
            accountExists = false;
            authId = string.Empty;
            Dictionary<string, Account> accounts = HttpRuntime.Cache[_accountKey] as Dictionary<string, Account>;
            if (!accounts.ContainsKey(account.Email))
            {
                accounts[account.Email] = account;
                authId = _authenticationProvider.CreateAuthenticationId(accounts[account.Email].AccountId);
                return true;
            }
            accountExists = true;
            return false;
        }

        public bool ForgotPassword(string accountId, out bool accountExists)
        {
            accountExists = true;
            return true;
        }

        public bool ForgotPasswordByEmail(string email, out string errorMessage)
        {
            errorMessage = string.Empty;
            return true;
        }

        public bool IsValid(string authenticationId)
        {
            return _authenticationProvider.Validate(authenticationId);
        }

        public string GetAccountIdByEmail(string email, string accountType = null)
        {
            Dictionary<string, Account> accounts = HttpRuntime.Cache[_accountKey] as Dictionary<string, Account>;
            if (accounts.ContainsKey(email))
                return accounts[email].AccountId;
            else
            {
                return null;
            }
        }

        public bool Exists(string accountId)
        {
            return true;
        }

        public Account GetAccount(string accountId)
        {
            Dictionary<string, Account> accounts = HttpRuntime.Cache[_accountKey] as Dictionary<string, Account>;
            foreach (var email in accounts.Keys)
            {
                if (accounts[email].AccountId == accountId)
                    return accounts[email];
            }
            return null;
        }

        public void SetEmailActivated(Account account)
        {
        }

        public void SetMobileVerified(Account account)
        {
        }


        public bool UpdatePassword(string accountId, string currentPassword, string newPassword)
        {
            return true;
        }


        public bool Exists(string socialAccountId, string socialAccountType)
        {
            return false;
        }

        public bool RegisterSocial(string socialAccountId, string socialAccountType, string email, string firstName,
                                   string lastName, string mobile, string ipAddress, out string authId,
                                   out bool accountExists)
        {
            authId = Guid.NewGuid().ToString();
            accountExists = true;
            return true;
        }

        public bool MergeSocial(string socialAccountId, string socialAccountType, string email)
        {
            return true;
        }

        public bool ChangeMobile(string accountId, string mobile)
        {
            return true;
        }

        public bool ChangeEmail(string accountId, string email)
        {
            return true;
        }

        public Account UpdatePersonalInfo(Account account, out string errorMessage)
        {
            errorMessage = string.Empty;
            return account;
        }


        public bool IsValid(string accountId, string emailCode)
        {
            return true;
        }

        public bool UpdatePassword(string accountId, string newPassword)
        {
            return true;
        }

        #endregion


        public BusOperator GetBusOperator(string accountId)
        {
            BusOperator busOperator = new BusOperator()
                                          {
                                              Addresses = new List<Address>()
                                                              {
                                                                  new Address()
                                                                      {
                                                                          Id = 1,
                                                                          AddressLine1 = "Address line one",
                                                                          AddressLine2 = "Address line two",
                                                                          AddressType = "Office",
                                                                          City = "Pune",
                                                                          Country = "India",
                                                                          State = "Maharashtra",
                                                                          ZipCode = "413512"
                                                                      }
                                                              },
                                              ComapanyName = "Amrutyog",
                                              BankAccount = new BankAccount()
                                                                {
                                                                    Id = 1,
                                                                    AccountNumber = "2343423423423",
                                                                    BankName = "HDFC account",
                                                                    Branch = "Kalyaninagar",
                                                                    IFSCCode = "JJF8768",
                                                                    AccountHolderName = "Mahesh Tokle",
                                                                    UserId = 1
                                                                },
                                              Email = new List<string>() {"mutokle@gmail.com"},
                                              PhoneNumber = new List<string>() {"7709161990"},
                                              OperatorId = 1
                                          };
            return busOperator;
        }

        public bool SaveOrUpdateBusOperator(BusOperator busOperator, out string errorMessage)
        {
            errorMessage = string.Empty;
            return true;
        }

        public bool SaveOrUpdateAddress(int accountId, Address address, out int addressId, out string errorMessage)
        {
            addressId = 1;
            errorMessage = string.Empty;
            return true;
        }

        public bool SaveOrUpdateBankAccount(BankAccount bankAccount, out int bankAccountId, out string errorMessage)
        {
            bankAccountId = 0;
            errorMessage = string.Empty;
            return true;
        }
    }
}

