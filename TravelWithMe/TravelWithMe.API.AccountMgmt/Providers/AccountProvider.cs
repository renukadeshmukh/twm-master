using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TravelWithMe.API.Interfaces;
using TravelWithMe.API.Core.Model;
using TravelWithMe.Data.Factories;
using TravelWithMe.Data.Interfaces;
using TravelWithMe.API.AccountMgmt.Utilities;
using TravelWithMe.API.Logging;
using TravelWithMe.Logging.Helper;
using TravelWithMe.API.Core.Model.Utilities;

namespace TravelWithMe.API.AccountMgmt.Providers
{
    public class AccountProvider : IAccountProvider
    {
        private const string Type = "Email";
        private const string Source = "AccountProvider";

        private readonly IAuthenticationProvider _authenticationProvider;

        private readonly ICaptchaProvider _captchaProvider;

        private readonly IEmailProvider _emailProvider;

        private readonly IMobileProvider _mobileProvider;

        public AccountProvider(IAuthenticationProvider authenticationProvider, ICaptchaProvider captchaProvider,
                               IEmailProvider emailProvider, IMobileProvider mobileProvider)
        {
            _authenticationProvider = authenticationProvider;
            _captchaProvider = captchaProvider;
            _emailProvider = emailProvider;
            _mobileProvider = mobileProvider;
        }

        #region IAccountProvider Members

        public Account Login(string email, string password, out string authId)
        {
            authId = null;

            string hashPwd = string.IsNullOrEmpty(password) ? null : HashGenerator.GenerateHash(password);

            IAccountDataProvider accountDataProvider = AccountDataProviderFactory.CreateAccountDataProvider();

            if (accountDataProvider != null)
            {
                Account account = accountDataProvider.GetAccount(email, hashPwd);
                if (account != null)
                {
                    authId = _authenticationProvider.CreateAuthenticationId(account.AccountId);
                    return account;
                }
            }
            return null;
        }

        public bool Register(Account account, out string authId, out bool accountExists)
        {
            authId = null;
            accountExists = true;
            string accountId = GetAccountIdByEmail(account.Email);
            if (!string.IsNullOrEmpty(accountId))
            {
                return false;
            }
            accountExists = false;
            string password = PasswordGenerator.GenerateRandomPassword();
            string hashPwd = HashGenerator.GenerateHash(password);
            IAccountDataProvider accountDataProvider = AccountDataProviderFactory.CreateAccountDataProvider();
            if (accountDataProvider != null)
            {
                //if(account.AccountType == AccountType.BusOperator)
                //{
                //    account.IsEnabled = false;
                //}
                //else
                {
                    account.IsEnabled = true;
                }
                account = accountDataProvider.CreateAccount(account, hashPwd);
                if (account != null)
                {
                    authId = _authenticationProvider.CreateAuthenticationId(account.AccountId);
                    try
                    {
                        _emailProvider.SendRegistrationEmail(account, password);
                    }
                    catch (Exception ex)
                    {
                        Logger.LogException(ex, Source, "SendRegistrationEmail", Severity.Major);
                    }
                    return true;
                }
            }
            return false;
        }

        public bool ForgotPasswordByEmail(string email, out string errorMessage)
        {
            errorMessage = string.Empty;
            try
            {
                string accountId = GetAccountIdByEmail(email);
                if (string.IsNullOrEmpty(accountId))
                {
                    errorMessage = string.Format("{0} account doesnt exist!!", email);
                    return false;
                }
                Account account = GetAccount(accountId);
                string password = PasswordGenerator.GenerateRandomPassword();
                string hashPwd = HashGenerator.GenerateHash(password);
                IAccountDataProvider accountDataProvider = AccountDataProviderFactory.CreateAccountDataProvider();
                if (accountDataProvider != null)
                {
                    if (accountDataProvider.SetPassword(accountId, hashPwd))
                    {
                        _emailProvider.SendForgotPasswordEmail(account.Email, account.FirstName, account.LastName, password);
                    }
                    else
                    {
                        errorMessage = "Faild to reset the password! Please report!";
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return false;
            }
            return true;
        }

        public bool RegisterSocial(string socialAccountId, string socialAccountType, string email, string firstName,
                                   string lastName, string mobile, string ipAddress, out string authId,
                                   out bool accountExists)
        {
            authId = null;
            accountExists = true;
            IAccountDataProvider accountDataProvider = AccountDataProviderFactory.CreateAccountDataProvider();
            Account account = null;
            if (accountDataProvider != null)
            {
                if (Exists(GetAccountIdByEmail(email)) || Exists(GetAccountIdForMobile(mobile)))
                {
                    account = accountDataProvider.GetAccount(GetAccountIdByEmail(email));
                }
                else
                {
                    accountExists = false;

                    string password = PasswordGenerator.GenerateRandomPassword();

                    string hashPwd = HashGenerator.GenerateHash(password);


                    account = accountDataProvider.CreateAccount(null, hashPwd);

                    try
                    {
                        _emailProvider.SendRegistrationEmail(account, password);
                    }
                    catch (Exception ex)
                    {
                        Logger.LogException(ex, Source, "SendRegistrationEmail", Severity.Major);
                    }
                }

                if (account != null)
                {
                    accountDataProvider.CreateSocialAccount(account.AccountId, socialAccountId, socialAccountType);

                    authId = _authenticationProvider.CreateAuthenticationId(account.AccountId);

                    if (!accountExists)
                    {
                        try
                        {
                            _mobileProvider.ResendMobileCode(account);
                        }
                        catch (Exception ex)
                        {
                            Logger.LogException(ex, Source, "ResendMobileCode", Severity.Major);
                        }
                    }
                    return true;
                }
            }
            return false;
        }

        public bool MergeSocial(string socialAccountId, string socialAccountType, string email)
        {
            string accountId = GetAccountIdByEmail(email);
            if (!string.IsNullOrEmpty(accountId) && Exists(accountId) && !Exists(socialAccountId, socialAccountType))
            {
                IAccountDataProvider accountDataProvider = AccountDataProviderFactory.CreateAccountDataProvider();

                if (accountDataProvider != null)
                {
                    accountDataProvider.CreateSocialAccount(accountId, socialAccountId, socialAccountType);
                    return true;
                }
            }
            return false;
        }

        public bool ForgotPassword(string accountId, out bool accountExists)
        {
            accountExists = false;
            if (Exists(accountId))
            {
                accountExists = true;
                Account account = GetAccount(accountId);

                string verificationCode = VerificationCodeGenerator.GenerateNewVerificationCode();
                ICodeVerificationDataProvider codeVerificationDataProvider =
                    CodeVerificationDataProviderFactory.CreateCodeVerificationDataProvider();

                codeVerificationDataProvider.SaveNewCode(account.AccountId, verificationCode, Type);

                try
                {
                    _emailProvider.SendForgotPasswordEmail(account.Email, account.FirstName, account.LastName,
                                                           verificationCode);
                }
                catch (Exception ex)
                {
                    Logger.LogException(ex, Source, "SendForgotPasswordEmail", Severity.Critical);
                    return false;
                }
                return true;
            }
            return false;
        }

        public bool IsValid(string authenticationId)
        {
            return _authenticationProvider.Validate(authenticationId);
        }

        public string GetAccountIdByEmail(string email, string accountType = null)
        {
            if (string.IsNullOrEmpty(email))
                return null;
            IAccountDataProvider accountDataProvider = AccountDataProviderFactory.CreateAccountDataProvider();
            if (string.IsNullOrEmpty(accountType))
            {
                return accountDataProvider.GetAccountIdByEmail(email);
            }
            return accountDataProvider.GetAccountIdForSocialAccount(email, accountType);
        }

        public bool Exists(string accountId)
        {
            if (string.IsNullOrEmpty(accountId))
                return false;
            IAccountDataProvider accountDataProvider = AccountDataProviderFactory.CreateAccountDataProvider();
            return accountDataProvider.Exists(accountId);
        }

        public bool Exists(string socialAccountId, string socialAccountType)
        {
            if (string.IsNullOrEmpty(socialAccountId))
                return false;
            IAccountDataProvider accountDataProvider = AccountDataProviderFactory.CreateAccountDataProvider();
            return accountDataProvider.Exists(socialAccountId, socialAccountType);
        }

        public Account GetAccount(string accountId)
        {
            if (string.IsNullOrEmpty(accountId))
                return null;
            IAccountDataProvider accountDataProvider = AccountDataProviderFactory.CreateAccountDataProvider();
            return accountDataProvider.GetAccount(accountId);
        }

        public void SetEmailActivated(Account account)
        {
            if (account != null)
            {
                IAccountDataProvider accountDataProvider = AccountDataProviderFactory.CreateAccountDataProvider();
                accountDataProvider.SetEmailActivated(account.Email);
            }
        }

        public void SetMobileVerified(Account account)
        {
            if (account != null)
            {
                IAccountDataProvider accountDataProvider = AccountDataProviderFactory.CreateAccountDataProvider();
                accountDataProvider.SetMobileVerified(account.Email);
            }
        }

        public bool UpdatePassword(string accountId, string currentPassword, string newPassword)
        {
            if (!string.IsNullOrEmpty(accountId))
            {
                IAccountDataProvider accountDataProvider = AccountDataProviderFactory.CreateAccountDataProvider();
                return accountDataProvider.UpdatePassword(accountId, HashGenerator.GenerateHash(currentPassword),
                                                          HashGenerator.GenerateHash(newPassword));
            }
            return false;
        }

        public bool ChangeMobile(string accountId, string mobile)
        {
            if (!string.IsNullOrEmpty(accountId) && !string.IsNullOrEmpty(mobile))
            {
                IAccountDataProvider accountDataProvider = AccountDataProviderFactory.CreateAccountDataProvider();
                return accountDataProvider.ChangeMobile(accountId, mobile);
            }
            return false;
        }

        public bool IsValid(string accountId, string emailCode)
        {
            ICodeVerificationDataProvider codeVerificationDataProvider =
                CodeVerificationDataProviderFactory.CreateCodeVerificationDataProvider();

            if (codeVerificationDataProvider.IsValid(accountId, emailCode, Type))
            {
                codeVerificationDataProvider.Remove(accountId, Type);
                return true;
            }
            return false;
        }

        public bool UpdatePassword(string accountId, string newPassword)
        {
            if (!string.IsNullOrEmpty(accountId))
            {
                IAccountDataProvider accountDataProvider = AccountDataProviderFactory.CreateAccountDataProvider();
                return accountDataProvider.SetPassword(accountId, HashGenerator.GenerateHash(newPassword));
            }
            return false;
        }

        public bool ChangeEmail(string accountId, string email)
        {
            if (!string.IsNullOrEmpty(accountId) && !string.IsNullOrEmpty(email))
            {
                IAccountDataProvider accountDataProvider = AccountDataProviderFactory.CreateAccountDataProvider();
                return accountDataProvider.ChangeEmail(accountId, email);
            }
            return false;
        }

        public Account UpdatePersonalInfo(Account account, out string errorMessage)
        {
            errorMessage = string.Empty;
            IAccountDataProvider accountDataProvider = AccountDataProviderFactory.CreateAccountDataProvider();
            if (accountDataProvider != null)
            {
                Account updatedAccount = accountDataProvider.UpdatePersonalInfo(account, out errorMessage);
                return updatedAccount;
            }
            return null;
        }

        public BusOperator GetBusOperator(string accountId)
        {
            if (string.IsNullOrEmpty(accountId))
                return null;
            int id = int.Parse(accountId);
            IAccountDataProvider accountDataProvider = AccountDataProviderFactory.CreateAccountDataProvider();
            return accountDataProvider.GetBusOperatorInfo(id);
        }

        public bool SaveOrUpdateBusOperator(BusOperator busOperator, out string errorMessage)
        {
            IAccountDataProvider accountDataProvider = AccountDataProviderFactory.CreateAccountDataProvider();
            if(accountDataProvider.SaveOrUpdateBusOperator(busOperator, out errorMessage))
            {
                return true;
            }
            return false;
        }

        public bool SaveOrUpdateAddress(int accountId, Address address, out int addressId, out string errorMessage)
        {
            IAccountDataProvider accountDataProvider = AccountDataProviderFactory.CreateAccountDataProvider();
            if (accountDataProvider.SaveOrUpdateAddress(accountId, address, out addressId, out errorMessage))
            {
                return true;
            }
            return false;
        }

        public bool SaveOrUpdateBankAccount(BankAccount bankAccount, out int bankAccountId, out string errorMessage)
        {
            IAccountDataProvider accountDataProvider = AccountDataProviderFactory.CreateAccountDataProvider();
            if (accountDataProvider.SaveOrUpdateBankAccount(bankAccount, out bankAccountId, out errorMessage))
            {
                return true;
            }
            return false;
        }
        #endregion

        public string GetAccountIdForMobile(string mobile)
        {
            if (string.IsNullOrEmpty(mobile))
                return null;

            IAccountDataProvider accountDataProvider = AccountDataProviderFactory.CreateAccountDataProvider();
            return accountDataProvider.GetAccountIdForMobile(mobile);
        }
    }
}
