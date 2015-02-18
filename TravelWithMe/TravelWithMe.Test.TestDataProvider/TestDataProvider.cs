using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TravelWithMe.API.AccountMgmt.Utilities;
using TravelWithMe.API.Core.Model.Utilities;
using TravelWithMe.API.AuthenticationMgmt.Providers;
using TravelWithMe.API.Interfaces;
using TravelWithMe.Data.MySql;
using TravelWithMe.API.Core.Caching.Http;
using TravelWithMe.API.Core.Model;

namespace TravelWithMe.Test.TestDataProvider
{
    public class TestDataProvider
    {
         private readonly AccountDataProvider _accountDataProvider;
        private readonly IAuthenticationProvider _authenticationProvider;
        private readonly ICacheProvider _cacheProvider;

        public TestDataProvider()
        {
            _cacheProvider = new HttpRuntimeCache();
            _authenticationProvider = new AuthenticationProvider(_cacheProvider);
            _accountDataProvider = new AccountDataProvider();
        }

        private User TestUser { get; set; }
        public User GetTestUser(string email = null)
        {
            email = string.IsNullOrEmpty(email) ? "tiatma@upcurve.in" : email;
            string password = PasswordGenerator.GenerateRandomPassword();
            string hashPwd = HashGenerator.GenerateHash(password);
            const string firstName = "TestFName";
            const string lastName = "TestLName";
            string mobile = RandomGenerator.GenerateRandomNumberString(10);
            string mobileCode = "123456"; //VerificationCodeGenerator.GenerateNewVerificationCode();
            string emailCode = VerificationCodeGenerator.GenerateNewVerificationCode();
            var testUser = new User
            {
                Email = email,
                Password = password,
                HashPassword = hashPwd,
                FirstName = firstName,
                LastName = lastName,
                Mobile = mobile,
                MobileCode = mobileCode,
                EmailCode = emailCode
            };
            return testUser;
        }

        public UserAccount CreateAccount(string email = null, string password = null)
        {
            email = string.IsNullOrEmpty(email) ? TestUser.Email : email;
            password = string.IsNullOrEmpty(password) ? TestUser.Password : password;
            string hashPwd = HashGenerator.GenerateHash(password);
            string mobile = RandomGenerator.GenerateRandomNumberString(10); //Since DB mobile number should be unique
            string firstName = TestUser != null ? TestUser.FirstName : "TestFName";
            string lastName = TestUser != null ? TestUser.LastName : "TestLName";
            
            Account account = _accountDataProvider.CreateAccount(new Account(){ Email = email, FirstName = firstName, LastName = lastName}, hashPwd) ?? 
                                    _accountDataProvider.GetAccount(_accountDataProvider.GetAccountIdByEmail(email));

            if (account != null)
            {
                string authenticationId = _authenticationProvider.CreateAuthenticationId(account.AccountId);
                var testUserAccount = new UserAccount
                {
                    AccountId = account.AccountId,
                    AuthenticationId = authenticationId
                };
                return testUserAccount;
            }
            throw new NullReferenceException("Account could not be created.");
        }

        public bool CleanUpTestContext(string accountId, string authenticationId)
        {
            Account existingAccount = _accountDataProvider.GetAccount(accountId);
            if (existingAccount != null)
            {
                bool deleteAccount = _accountDataProvider.DeleteAccount(accountId);
                bool logout = _authenticationProvider.Logout(authenticationId);
                return deleteAccount && logout;
            }
            return true;
        }
    }
}
