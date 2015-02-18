using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using TravelWithMe.API.Interfaces;
using TravelWithMe.API.Logging;
using TravelWithMe.Logging.Helper;
using TravelWithMe.API.Core.Model;
using TravelWithMe.API.Core.Model.Utilities;
using TravelWithMe.Data.Factories;
using TravelWithMe.Data.Interfaces;

namespace TravelWithMe.API.EmailMgmt.Providers
{
    public class EmailProvider : IEmailProvider
    {
        private const string Type = "Email";
        private string Source = "EmailProvider";

        #region IEmailProvider Members

        public void SendRegistrationEmail(Account account, string password)
        {
            try
            {
                string contentBody = EmailTemplateHelper.RegistrationTemplate;
                contentBody = contentBody.Replace(EmailTemplateHelper.Keys.Key_User, account.FirstName);
                contentBody = contentBody.Replace(EmailTemplateHelper.Keys.Key_Email, account.Email);
                contentBody = contentBody.Replace(EmailTemplateHelper.Keys.Key_Password, password);
                contentBody = contentBody.Replace(EmailTemplateHelper.Keys.Key_Year, DateTime.Today.Year.ToString());
                contentBody = contentBody.Replace(EmailTemplateHelper.Keys.Key_BaseSiteUrl, ConfigHelper.BaseSiteUrl);
                SendEmail(ConfigHelper.CustomerCareEmail, account.Email, "Welcome to BusSwitch", contentBody,"SendRegistrationEmail");
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, Source, "SendRegistrationEmail", Severity.Critical);
            }
        }

        public void SendForgotPasswordEmail(string toEmail, string firstName, string lastName, string password)
        {
            try
            {
                string contentBody = EmailTemplateHelper.ForgotPasswordTemplate;
                contentBody = contentBody.Replace(EmailTemplateHelper.Keys.Key_BaseSiteUrl, ConfigHelper.BaseSiteUrl);
                contentBody = contentBody.Replace(EmailTemplateHelper.Keys.Key_Email, toEmail);
                contentBody = contentBody.Replace(EmailTemplateHelper.Keys.Key_Password, password);

                SendEmail(ConfigHelper.CustomerCareEmail, toEmail, "Did you forget your password?", contentBody,
                          "SendForgotPasswordEmail");
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, Source, "SendForgotPasswordEmail", Severity.Critical);
            }
        }


        public void SendWLEmail(string email, string requestId, string friendlyRequestId, DateTime travelDate,
                                string fromCity, string toCity, List<Passenger> passengers)
        {
            try
            {
                {
                    string contentBody = EmailTemplateHelper.RequestSubmittedTemplate;
                    contentBody = contentBody.Replace(EmailTemplateHelper.Keys.Key_RequestFriendlyName,
                                                      friendlyRequestId);
                    contentBody = contentBody.Replace(EmailTemplateHelper.Keys.Key_From, fromCity);
                    contentBody = contentBody.Replace(EmailTemplateHelper.Keys.Key_To, toCity);
                    contentBody = contentBody.Replace(EmailTemplateHelper.Keys.Key_Date, travelDate.ToShortDateString());
                    contentBody = contentBody.Replace(EmailTemplateHelper.Keys.Key_Year, DateTime.Today.Year.ToString());

                    string travellersContent = string.Empty;

                    foreach (Passenger passenger in passengers)
                    {
                        string travellerContent = EmailTemplateHelper.TravellerTemplate;
                        travellerContent = travellerContent.Replace(EmailTemplateHelper.Keys.Key_Traveller,
                                                                    string.Format("{0} {1} {2}", passenger.Title,
                                                                                  passenger.FirstName,
                                                                                  passenger.LastName));
                        travellersContent += travellerContent;
                    }

                    contentBody = contentBody.Replace(EmailTemplateHelper.Keys.Key_Travellers, travellersContent);

                    SendEmail(ConfigHelper.CustomerCareEmail, email,
                              "BusSwitch: Your request " + friendlyRequestId + " has been registered.", contentBody,
                              "SendWLEmail");
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, Source, "SendWLEmail", Severity.Critical);
            }
        }

        public bool VerifyEmail(Account account, string emailCode)
        {
            try
            {
                if (account != null)
                {
                    ICodeVerificationDataProvider codeVerificationDataProvider =
                        CodeVerificationDataProviderFactory.CreateCodeVerificationDataProvider();

                    if (codeVerificationDataProvider.IsValid(account.AccountId, emailCode, Type))
                    {
                        codeVerificationDataProvider.Remove(account.AccountId, Type);
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, Source, "VerifyEmail", Severity.Critical);
            }
            return false;
        }

        public void ResendEmailVerification(Account account)
        {
            string verificationCode = GetVerificationCode(account);

            try
            {
                string contentBody = EmailTemplateHelper.EmailVerificationTemplate;
                contentBody = contentBody.Replace(EmailTemplateHelper.Keys.Key_EmailVerification_Url,
                                                  string.Format(ConfigHelper.EmailVerificationBaseUrl, account.Email,
                                                                verificationCode));

                SendEmail(ConfigHelper.CustomerCareEmail, account.Email,
                          "BusSwitch Email Verification.", contentBody, "ResendEmailVerification");
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, Source, "ResendEmailVerification", Severity.Critical);
            }
        }


        public void SendWLResultsEmail(string email, string requestId, DateTime travelDate, string fromCity,
                                       string toCity, List<Passenger> passengers)
        {
            try
            {
                string contentBody = EmailTemplateHelper.RequestDealsTemplate;
                contentBody = contentBody.Replace(EmailTemplateHelper.Keys.Key_DealsUrl,
                                                  string.Format(ConfigHelper.RedirectUrl
                                                                , requestId));
                contentBody = contentBody.Replace(EmailTemplateHelper.Keys.Key_Time, "6:00 PM");
                contentBody = contentBody.Replace(EmailTemplateHelper.Keys.Key_From, fromCity);
                contentBody = contentBody.Replace(EmailTemplateHelper.Keys.Key_To, toCity);
                contentBody = contentBody.Replace(EmailTemplateHelper.Keys.Key_Date, travelDate.ToShortDateString());

                string travellersContent = string.Empty;

                foreach (Passenger passenger in passengers)
                {
                    string travellerContent = EmailTemplateHelper.TravellerTemplate;
                    travellerContent = travellerContent.Replace(EmailTemplateHelper.Keys.Key_Traveller,
                                                                string.Format("{0} {1} {2}", passenger.Title,
                                                                              passenger.FirstName,
                                                                              passenger.LastName));
                    travellersContent += travellerContent;
                }

                contentBody = contentBody.Replace(EmailTemplateHelper.Keys.Key_Travellers, travellersContent);

                contentBody = contentBody.Replace(EmailTemplateHelper.Keys.Key_Year, DateTime.Today.Year.ToString());

                SendEmail(ConfigHelper.CustomerCareEmail, email, "BusSwitch: Great deals are here.", contentBody,
                          "SendWLResultsEmail");
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, Source, "SendWLResultsEmail", Severity.Critical);
            }
        }

        #endregion

        private void SendEmail(string from, string to, string subject, string content, string method)
        {
            Task.Factory.StartNew(() =>
                                      {
                                          try
                                          {
                                              using (
                                                  var smtpClient = new SmtpClient(ConfigHelper.SmtpEmailHost,
                                                                                  ConfigHelper.SmtpPort)
                                                                       {
                                                                           UseDefaultCredentials = false,
                                                                           Credentials =
                                                                               new NetworkCredential(
                                                                               ConfigHelper.CustomerCareUsername,
                                                                               ConfigHelper.CustomerCarePassword),
                                                                           EnableSsl = ConfigHelper.IsSmtpSslEnabled,
                                                                           Timeout = 10000,
                                                                           DeliveryMethod = SmtpDeliveryMethod.Network
                                                                       })
                                              {
                                                  var mailMessage = new MailMessage(from, to,
                                                                                    subject, content)
                                                                        {
                                                                            BodyEncoding = Encoding.UTF8,
                                                                            DeliveryNotificationOptions =
                                                                                DeliveryNotificationOptions.OnFailure,
                                                                            IsBodyHtml = true
                                                                        };
                                                  smtpClient.Send(mailMessage);
                                              }
                                          }
                                          catch (Exception ex)
                                          {
                                              Logger.LogException(ex, Source, method, Severity.Critical);
                                          }
                                      });
        }

        private static string GetVerificationCode(Account account)
        {
            string verificationCode = VerificationCodeGenerator.GenerateNewVerificationCode();
            ICodeVerificationDataProvider codeVerificationDataProvider =
                CodeVerificationDataProviderFactory.CreateCodeVerificationDataProvider();

            codeVerificationDataProvider.SaveNewCode(account.AccountId, verificationCode, Type);
            return verificationCode;
        }
    }
}