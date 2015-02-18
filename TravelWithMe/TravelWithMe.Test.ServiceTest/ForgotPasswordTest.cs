using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TravelWithMe.Test.TestDataProvider;
using TravelWithMe.API.Services.ServiceContract;
using TravelWithMe.API.Services.ServiceImplementation;
using TravelWithMe.API.Services.DataContract;

namespace TravelWithMe.Test.ServiceTest
{
    [TestClass]
    public class ForgotPasswordTest
    {
        private static string _sessionId;
        private static TestDataProvider.TestDataProvider TestDataProvider { get; set; }
        private static User TestUser { get; set; }
        private static UserAccount TestUserAccount { get; set; }

        private IAccountService AccountService { get; set; }

        [ClassInitialize]
        public static void ClassInit(TestContext testContext)
        {
            _sessionId = Guid.NewGuid().ToString();
            TestDataProvider = new TestDataProvider.TestDataProvider();
            TestUser = TestDataProvider.GetTestUser("forgotpasswordtiatma@upcurve.in");
            TestUserAccount = TestDataProvider.CreateAccount(TestUser.Email, TestUser.Password);
        }

        [TestInitialize]
        public void Initialize()
        {
            AccountService = new AccountService();
        }

        [TestMethod]
        [TestCategory("Smoke")]
        public void ForgotPasswordByEmailTest()
        {
            ForgotPasswordResponse response = AccountService.ForgotPasswordByEmail(_sessionId, TestUser.Email);
            Assert.IsNotNull(response, "Forgot Password response is null");
            Assert.IsTrue(response.IsSuccess, "Forgot Password response returns not successful");
        }

        [ClassCleanup]
        public static void CleanUp()
        {
            TestDataProvider.CleanUpTestContext(TestUserAccount.AccountId, TestUserAccount.AuthenticationId);
        }
    }
}
