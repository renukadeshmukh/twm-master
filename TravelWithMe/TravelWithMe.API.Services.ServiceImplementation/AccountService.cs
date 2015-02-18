using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TravelWithMe.API.Services.DataContract;
using TravelWithMe.API.Services.ServiceContract;
using TravelWithMe.API.Logging;
using System.ServiceModel.Activation;
using TravelWithMe.API.Core.Infra;
using TravelWithMe.API.Interfaces;
using TravelWithMe.Logging.Helper;
using TravelWithMe.API.Core.Factories;
using Model = TravelWithMe.API.Core.Model;

namespace TravelWithMe.API.Services.ServiceImplementation
{
    [ServerMessageLogger]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class AccountService : IAccountService
    {
        private const string Source = "AccountService";

        #region IAccountService Members
        public LoginResponse Login(string sessionId, LoginRequest request)
        {
            var response = new LoginResponse();
            using (new ApplicationContextScope(new ApplicationContext()))
            {
                ApplicationContext.SetSessionId(sessionId);
                string authId = string.Empty;
                try
                {
                    IAccountProvider accountProvider = AccountProviderFactory.GetAccountProvider();
                    Model.Account account = accountProvider.Login(request.Email, request.Password, out authId);
                    if (account != null)
                    {
                        response.Account = account.ToDataContract();
                        #region Save account in session
                        ISessionProvider sessionProvider = SessionProviderFactory.GetSessionProvider();
                        Model.SessionData data = sessionProvider.GetSession(sessionId);
                        data.UserAccount = account;
                        sessionProvider.SaveSession(data);
                        #endregion
                        response.IsSuccess = true;
                    }
                    else
                    {
                        response.IsSuccess = false;
                        response.ErrorMessage = "Invalid Email/Password. Please try again.";
                    }
                }
                catch (Exception exception)
                {
                    response.IsSuccess = false;
                    response.ErrorMessage = "Something is not quite right here. Please try again later.";
                    Logger.LogException(exception, Source, "Login", Severity.Critical);
                }
                response.AuthenticationId = authId;
            }
            return response;
        }

        public RegisterResponse Register(string sessionId, Account account)
        {
            var response = new RegisterResponse();
            using (new ApplicationContextScope(new ApplicationContext()))
            {
                ApplicationContext.SetSessionId(sessionId);
                try
                {
                    bool accountExists;
                    string authId;
                    IAccountProvider accountProvider = AccountProviderFactory.GetAccountProvider();
                    Model.Account modelAccount = account.ToDataModel();
                    if (accountProvider.Register(modelAccount , out authId, out accountExists))
                    {
                        ISessionProvider sessionProvider = SessionProviderFactory.GetSessionProvider();
                        Model.SessionData data = sessionProvider.GetSession(sessionId);
                        data.UserAccount = modelAccount;
                        sessionProvider.SaveSession(data);
                        response.Account = account;
                        response.IsSuccess = true;
                        response.AuthenticationId = authId;
                    }
                    else if (accountExists)
                    {
                        response.ErrorMessage = account.Email +
                                                " username already exists. Please login or click forgot password to reset the password.";
                    }
                }
                catch (Exception exception)
                {
                    response.ErrorMessage = "Something is not quite right here. Please try again later.";
                    Logger.LogException(exception, Source, "Register", Severity.Critical);
                }
            }
            return response;
        }

        public GetBusOperatorInfoResponse GetBusOperatorInfo(string sessionId, string authenticationId)
        {
            var response = new GetBusOperatorInfoResponse();
            using (new ApplicationContextScope(new ApplicationContext()))
            {
                ApplicationContext.SetSessionId(sessionId);
                try
                {
                    IAuthenticationProvider authProvider = AuthenticationProviderFactory.GetAuthenticationProvider();
                    if (authProvider.Validate(authenticationId))
                    {
                        string accountId = authProvider.GetAccountId(authenticationId);
                        ISessionProvider sessionProvider = SessionProviderFactory.GetSessionProvider();
                        if (sessionProvider.IsBusOperator(sessionId, accountId))
                        {
                            IAccountProvider accountProvider = AccountProviderFactory.GetAccountProvider();
                            Model.BusOperator busOperator = accountProvider.GetBusOperator(accountId);
                            if (busOperator!=null)
                            {
                                response.BusOperator = busOperator.ToDataContract();
                                response.IsSuccess = true;
                            }
                            else
                            {
                                response.IsSuccess = false;
                            }
                        }
                    }
                }
                catch (Exception exception)
                {
                    response.ErrorMessage = "Something is not quite right here. Please try again later.";
                    Logger.LogException(exception, Source, "Register", Severity.Critical);
                }
            }
            return response;    
        }

        public SaveBusOperatorResponse SaveBankOperatorInfo(string sessionId, string authenticationId, BusOperator busOperator)
        {
            var response = new SaveBusOperatorResponse();
            using (new ApplicationContextScope(new ApplicationContext()))
            {
                ApplicationContext.SetSessionId(sessionId);
                try
                {
                    IAuthenticationProvider authProvider = AuthenticationProviderFactory.GetAuthenticationProvider();
                    if (authProvider.Validate(authenticationId))
                    {
                        string id = authProvider.GetAccountId(authenticationId);
                        int accountId = int.Parse(id);
                        ISessionProvider sessionProvider = SessionProviderFactory.GetSessionProvider();
                        if (sessionProvider.IsBusOperator(sessionId, id))
                        {
                            IAccountProvider accountProvider = AccountProviderFactory.GetAccountProvider();
                            string errorMessage = string.Empty;
                            int addressId = 0;
                            int bankAccountId = 0;
                            Model.BusOperator busOperatorModel = busOperator.ToDataModel();
                            busOperatorModel.OperatorId = accountId;
                            if(accountProvider.SaveOrUpdateBusOperator(busOperatorModel, out errorMessage))
                            {
                                if(busOperatorModel.Addresses != null)
                                {
                                    int index = 0;
                                    foreach (Model.Address address in busOperatorModel.Addresses)
                                    {
                                        if(!accountProvider.SaveOrUpdateAddress(accountId, address, out addressId, out errorMessage))
                                        {
                                            response.AddressSaveFailures = response.AddressSaveFailures ?? new List<FailedObjects>();
                                            response.AddressSaveFailures.Add(new FailedObjects() { Index = index, ErrorMessage = errorMessage });
                                        }
                                        index++;
                                    }
                                }
                                if(busOperatorModel.BankAccount != null)
                                {
                                    busOperatorModel.BankAccount.UserId = accountId;
                                    if(!accountProvider.SaveOrUpdateBankAccount(busOperatorModel.BankAccount, out bankAccountId, out errorMessage))
                                    {
                                        response.BankAccountSaveFailure = errorMessage;
                                    }
                                }
                                response.IsSuccess = true;
                            }
                            else
                            {
                                response.IsSuccess = false;
                                response.ErrorMessage = errorMessage;
                            }
                        }
                    }
                }
                catch (Exception exception)
                {
                    response.ErrorMessage = "Something is not quite right here. Please try again later.";
                    Logger.LogException(exception, Source, "Register", Severity.Critical);
                }
            }
            return response;   
        }

        public bool ChangePassword(string sessionId, string authenticationId, ChangePasswordRequest request)
        {
            using (new ApplicationContextScope(new ApplicationContext()))
            {
                ApplicationContext.SetSessionId(sessionId);
                try
                {
                    IAuthenticationProvider authProvider = AuthenticationProviderFactory.GetAuthenticationProvider();
                    if (authProvider.Validate(authenticationId))
                    {
                        string accountId = authProvider.GetAccountId(authenticationId);
                        IAccountProvider accountProvider = AccountProviderFactory.GetAccountProvider();
                        return accountProvider.UpdatePassword(accountId, request.CurrentPassword, request.NewPassword);
                    }
                }
                catch (Exception exception)
                {
                    Logger.LogException(exception, Source, "ChangePassword", Severity.Critical);
                }
            }
            return false;
        }

        public ForgotPasswordResponse ForgotPasswordByEmail(string sessionId, string email)
        {
            var response = new ForgotPasswordResponse();
            using (new ApplicationContextScope(new ApplicationContext()))
            {
                ApplicationContext.SetSessionId(sessionId);
                try
                {
                    bool accountExists;
                    IAccountProvider accountProvider = AccountProviderFactory.GetAccountProvider();
                    string errorMessage = string.Empty;
                    if (accountProvider.ForgotPasswordByEmail(email, out errorMessage))
                    {
                        response.IsSuccess = true;
                    }
                    else
                    {
                        response.ErrorMessage = string.IsNullOrEmpty(errorMessage)
                                                    ? "Unknown error! Please report"
                                                    : errorMessage;
                    }
                }
                catch (Exception exception)
                {
                    response.ErrorMessage = "Something is not quite right here. Please try again later.";
                    Logger.LogException(exception, Source, "ForgotPasswordByEmail", Severity.Critical);
                }
            }
            return response;
        }

        public ChangeEmailResponse ChangeEmail(string sessionId, string authenticationId, string emailId)
        {
            var response = new ChangeEmailResponse { IsSuccess = true };
            using (new ApplicationContextScope(new ApplicationContext()))
            {
                ApplicationContext.SetSessionId(sessionId);
                try
                {
                    IAccountProvider accountProvider = AccountProviderFactory.GetAccountProvider();
                    IAuthenticationProvider authenticationProvider =
                        AuthenticationProviderFactory.GetAuthenticationProvider();
                    IEmailProvider verificationProvider = EmailProviderFactory.GetEmailProvider();

                    Core.Model.Account account =
                        accountProvider.GetAccount(authenticationProvider.GetAccountId(authenticationId));
                    if (account != null)
                    {
                        if (accountProvider.ChangeEmail(account.AccountId, emailId))
                        {
                            account.Email = emailId;
                            verificationProvider.ResendEmailVerification(account);
                            response.IsSuccess = true;
                        }
                        else
                        {
                            response.ErrorMessage =
                                "EmailId could not be changed. Please retry with valid information.";
                            response.IsSuccess = false;
                        }
                    }
                    else
                    {
                        response.ErrorMessage = "Invalid Account or EmailId. Please retry with valid information.";
                        response.IsSuccess = false;
                    }
                }
                catch (Exception exception)
                {
                    response.ErrorMessage = "Something is not quite right here. Please try again later.";
                    response.IsSuccess = false;
                    Logger.LogException(exception, Source, "ResendMobileCode", Severity.Critical);
                }
            }
            return response;
        }

        public ChangePersonalInfoResponse ChangePersonalInfo(string sessionId, string authenticationId, Account account)
        {
            var response = new ChangePersonalInfoResponse { IsSuccess = true };
            using (new ApplicationContextScope(new ApplicationContext()))
            {
                ApplicationContext.SetSessionId(sessionId);
                try
                {
                    IAccountProvider accountProvider = AccountProviderFactory.GetAccountProvider();
                    IAuthenticationProvider authenticationProvider =
                        AuthenticationProviderFactory.GetAuthenticationProvider();
                    IEmailProvider verificationProvider = EmailProviderFactory.GetEmailProvider();

                    Core.Model.Account authenticatedAccount =
                        accountProvider.GetAccount(authenticationProvider.GetAccountId(authenticationId));
                    if (authenticatedAccount != null)
                    {
                        string errorMessage = null;
                        authenticatedAccount.FirstName = account.FirstName;
                        authenticatedAccount.LastName = account.LastName;
                        Core.Model.Account updatedAccount = accountProvider.UpdatePersonalInfo(authenticatedAccount,
                                                                                               out errorMessage);
                        if (updatedAccount == null)
                        {
                            response.ErrorMessage = !string.IsNullOrEmpty(errorMessage)
                                                        ? errorMessage
                                                        : "Not able to save your information. Please retry with valid information.";
                            response.IsSuccess = false;
                        }
                    }
                    else
                    {
                        response.ErrorMessage = "Invalid Account or EmailId. Please retry with valid information.";
                        response.IsSuccess = false;
                    }
                }
                catch (Exception exception)
                {
                    response.ErrorMessage = "Something is not quite right here. Please try again later.";
                    response.IsSuccess = false;
                    Logger.LogException(exception, Source, "ChangePersonalInfoResponse", Severity.Critical);
                }
            }
            return response;
        }

        public bool Logout(string sessionId, string authenticationId)
        {
            using (new ApplicationContextScope(new ApplicationContext()))
            {
                ApplicationContext.SetSessionId(sessionId);
                try
                {
                    IAuthenticationProvider authProvider = AuthenticationProviderFactory.GetAuthenticationProvider();
                    return authProvider.Logout(authenticationId);
                }
                catch (Exception exception)
                {
                    Logger.LogException(exception, Source, "Logout", Severity.Critical);
                }
            }
            return false;
        }

        public LoginResponse Validate(string sessionId, string authenticationId)
        {
            var response = new LoginResponse();
            using (new ApplicationContextScope(new ApplicationContext()))
            {
                ApplicationContext.SetSessionId(sessionId);
                try
                {
                    IAuthenticationProvider authProvider = AuthenticationProviderFactory.GetAuthenticationProvider();
                    if (authProvider.Validate(authenticationId))
                    {
                        response.AuthenticationId = authenticationId;
                        IAccountProvider accountProvider = AccountProviderFactory.GetAccountProvider();
                        try
                        {
                            Model.Account account = accountProvider.GetAccount(authProvider.GetAccountId(authenticationId));
                            if (account != null)
                            {
                                response.Account = account.ToDataContract();
                                response.IsSuccess = true;
                            }
                            else
                            {
                                response.ErrorMessage = "Invalid Email/Password. Please try again.";
                            }
                        }
                        catch (Exception exception)
                        {
                            response.ErrorMessage = "Something is not quite right here. Please try again later.";
                            Logger.LogException(exception, Source, "Login", Severity.Critical);
                        }
                        return response;
                    }
                    else
                    {
                        response.ErrorMessage = "Session Invalid. Login again.";
                    }
                }
                catch (Exception exception)
                {
                    Logger.LogException(exception, Source, "Validate", Severity.Critical);
                }
            }
            return response;
        }


        public RegisterResponse RegisterSocial(string sessionId, string socialAccountId, string socialAccountType,
                                               string email, string firstName, string lastName, string mobile,
                                               string ipAddress)
        {
            var response = new RegisterResponse();
            using (new ApplicationContextScope(new ApplicationContext()))
            {
                ApplicationContext.SetSessionId(sessionId);

                try
                {
                    bool accountExists;
                    string authId;
                    bool isCaptchaValid;
                    IAccountProvider accountProvider = AccountProviderFactory.GetAccountProvider();

                    if (accountProvider.RegisterSocial(socialAccountId, socialAccountType, email, firstName, lastName,
                                                       mobile, ipAddress,
                                                       out authId, out accountExists))
                    {
                        response.IsSuccess = true;
                        response.AuthenticationId = authId;
                    }
                    else if (accountExists)
                    {
                        response.ErrorMessage = email +
                                                " username already exists. Please login or click forgot password to reset the password.";
                    }
                }
                catch (Exception exception)
                {
                    response.ErrorMessage = "Something is not quite right here. Please try again later.";
                    Logger.LogException(exception, Source, "Register", Severity.Critical);
                }
            }
            return response;
        }

        public bool MergeSocial(string sessionId, string socialAccountId, string socialAccountType, string email)
        {
            using (new ApplicationContextScope(new ApplicationContext()))
            {
                ApplicationContext.SetSessionId(sessionId);
                string authId = string.Empty;
                IAccountProvider accountProvider = AccountProviderFactory.GetAccountProvider();
                try
                {
                    return accountProvider.MergeSocial(socialAccountId, socialAccountType, email);
                }
                catch (Exception exception)
                {
                    Logger.LogException(exception, Source, "Login", Severity.Critical);
                }
            }
            return false;
        }

        public bool VerifyMobile(string sessionId, string authenticationId, string mobileCode)
        {
            bool response = false;
            using (new ApplicationContextScope(new ApplicationContext()))
            {
                ApplicationContext.SetSessionId(sessionId);
                try
                {
                    IAccountProvider accountProvider = AccountProviderFactory.GetAccountProvider();
                    IAuthenticationProvider authenticationProvider =
                        AuthenticationProviderFactory.GetAuthenticationProvider();

                    Core.Model.Account account =
                        accountProvider.GetAccount(authenticationProvider.GetAccountId(authenticationId));

                    if (account != null)
                    {
                        IMobileProvider verificationProvider = MobileProviderFactory.GetMobileProvider();

                        response = verificationProvider.VerifyMobile(account, mobileCode);
                        if (response)
                        {
                            accountProvider.SetMobileVerified(account);
                        }
                    }
                }
                catch (Exception exception)
                {
                    Logger.LogException(exception, Source, "VerifyMobile", Severity.Critical);
                }
            }
            return response;
        }

        public bool VerifyEmail(string sessionId, string email, string emailCode)
        {
            bool response = false;
            using (new ApplicationContextScope(new ApplicationContext()))
            {
                ApplicationContext.SetSessionId(sessionId);
                try
                {
                    IAccountProvider accountProvider = AccountProviderFactory.GetAccountProvider();
                    Core.Model.Account account = accountProvider.GetAccount(accountProvider.GetAccountIdByEmail(email));
                    if (account != null)
                    {
                        IEmailProvider verificationProvider = EmailProviderFactory.GetEmailProvider();

                        response = verificationProvider.VerifyEmail(account, emailCode);
                        if (response)
                        {
                            accountProvider.SetEmailActivated(account);
                        }
                    }
                }
                catch (Exception exception)
                {
                    Logger.LogException(exception, Source, "VerifyEmail", Severity.Critical);
                }
            }
            return response;
        }

        public ResendMobileCodeResponse ResendMobileCode(string sessionId, string authenticationId)
        {
            var response = new ResendMobileCodeResponse();
            using (new ApplicationContextScope(new ApplicationContext()))
            {
                ApplicationContext.SetSessionId(sessionId);
                try
                {
                    IAccountProvider accountProvider = AccountProviderFactory.GetAccountProvider();
                    IAuthenticationProvider authenticationProvider =
                        AuthenticationProviderFactory.GetAuthenticationProvider();
                    Core.Model.Account account =
                        accountProvider.GetAccount(authenticationProvider.GetAccountId(authenticationId));
                    if (account != null)
                    {
                        IMobileProvider verificationProvider = MobileProviderFactory.GetMobileProvider();

                        verificationProvider.ResendMobileCode(account);
                        response.IsSuccess = true;
                    }
                    else
                    {
                        response.ErrorMessage = "Invalid Account or Mobile number. Please retry with valid information.";
                    }
                }
                catch (Exception exception)
                {
                    response.ErrorMessage = "Something is not quite right here. Please try again later.";
                    Logger.LogException(exception, Source, "ResendMobileCode", Severity.Critical);
                }
            }
            return response;
        }

        public ResendEmailCodeResponse ResendEmailVerification(string sessionId, string authenticationId)
        {
            var response = new ResendEmailCodeResponse();
            using (new ApplicationContextScope(new ApplicationContext()))
            {
                ApplicationContext.SetSessionId(sessionId);
                try
                {
                    IAccountProvider accountProvider = AccountProviderFactory.GetAccountProvider();
                    IAuthenticationProvider authenticationProvider =
                        AuthenticationProviderFactory.GetAuthenticationProvider();

                    Core.Model.Account account =
                        accountProvider.GetAccount(authenticationProvider.GetAccountId(authenticationId));

                    if (account != null)
                    {
                        IEmailProvider verificationProvider = EmailProviderFactory.GetEmailProvider();

                        verificationProvider.ResendEmailVerification(account);
                        response.IsSuccess = true;
                    }
                    else
                    {
                        response.ErrorMessage = "Invalid Account or Email address. Please retry with valid information.";
                    }
                }
                catch (Exception exception)
                {
                    response.ErrorMessage = "Something is not quite right here. Please try again later.";
                    Logger.LogException(exception, Source, "ResendMobileCode", Severity.Critical);
                }
            }
            return response;
        }

        

        public GetAccountResponse GetAccount(string sessionId, string authenticationId)
        {
            var response = new GetAccountResponse { IsSuccess = true };
            using (new ApplicationContextScope(new ApplicationContext()))
            {
                ApplicationContext.SetSessionId(sessionId);
                try
                {
                    IAuthenticationProvider authProvider = AuthenticationProviderFactory.GetAuthenticationProvider();
                    if (authProvider.Validate(authenticationId))
                    {
                        string accountId = authProvider.GetAccountId(authenticationId);
                        IAccountProvider accountProvider = AccountProviderFactory.GetAccountProvider();
                        response.UserAccount = accountProvider.GetAccount(accountId).ToDataContract();
                        return response;
                    }
                }
                catch (Exception exception)
                {
                    response.IsSuccess = false;
                    response.ErrorMessage = exception.Message;
                    Logger.LogException(exception, Source, "GetAccount", Severity.Critical);
                }
            }
            return null;
        }

        

        public bool ResetPassword(string sessionId, string email, string emailCode, string newPassword)
        {
            using (new ApplicationContextScope(new ApplicationContext()))
            {
                ApplicationContext.SetSessionId(sessionId);

                try
                {
                    IAccountProvider accountProvider = AccountProviderFactory.GetAccountProvider();
                    string accountId = accountProvider.GetAccountIdByEmail(email);

                    if (!string.IsNullOrEmpty(accountId) && accountProvider.IsValid(accountId, emailCode))
                    {
                        return accountProvider.UpdatePassword(accountId, newPassword);
                    }
                }
                catch (Exception exception)
                {
                    Logger.LogException(exception, Source, "ResetPassword", Severity.Critical);
                }
            }
            return false;
        }
        #endregion
    }
}
