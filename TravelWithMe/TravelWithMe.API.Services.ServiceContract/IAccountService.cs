using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Web;
using TravelWithMe.API.Services.DataContract;
using TravelWithMe.API.Services.DataContract.Messages;

namespace TravelWithMe.API.Services.ServiceContract
{
    [ServiceContract(Name = "IAccountServiceRest", Namespace = "http://www.busswitch.in/Services/2012/08")]
    public interface IAccountService
    {
        [OperationContract]
        [WebInvoke(
            UriTemplate =
                "/ChangePersonalInfo/{authenticationId}?session={sessionId}",
            BodyStyle = WebMessageBodyStyle.Bare)]
        ChangePersonalInfoResponse ChangePersonalInfo(string sessionId, string authenticationId, Account account);

        [OperationContract]
        [WebGet(
            UriTemplate =
                "/ChangeEmail/{authenticationId}?session={sessionId}&email={emailId}",
            BodyStyle = WebMessageBodyStyle.Bare)]
        ChangeEmailResponse ChangeEmail(string sessionId, string authenticationId, string emailId);

        [OperationContract]
        [WebGet(
            UriTemplate =
                "/GetBusOperatorInfo/{authenticationId}?session={sessionId}",
            BodyStyle = WebMessageBodyStyle.Bare)]
        GetBusOperatorInfoResponse GetBusOperatorInfo(string sessionId, string authenticationId);

        [OperationContract]
        [WebInvoke(
            UriTemplate =
                "/SaveBankOperatorInfo/{authenticationId}?session={sessionId}",
            BodyStyle = WebMessageBodyStyle.Bare)]
        SaveBusOperatorResponse SaveBankOperatorInfo(string sessionId, string authenticationId, BusOperator busOperator);

        [OperationContract]
        [WebInvoke(
            UriTemplate =
                "/register?session={sessionId}",
            BodyStyle = WebMessageBodyStyle.Bare)]
        RegisterResponse Register(string sessionId, Account account);

        [OperationContract]
        [WebInvoke(
            UriTemplate =
                "/login?session={sessionId}",
            BodyStyle = WebMessageBodyStyle.Bare)]
        LoginResponse Login(string sessionId, LoginRequest request);

        [OperationContract]
        [WebGet(
            UriTemplate =
                "/registersocial?session={sessionId}&socialAccountId={socialAccountId}&socialAccountType={socialAccountType}&email={email}&firstName={firstName}&lastName={lastName}&mobile={mobile}&ipAddress={ipAddress}",
            BodyStyle = WebMessageBodyStyle.Bare)]
        RegisterResponse RegisterSocial(string sessionId, string socialAccountId, string socialAccountType, string email,
                                        string firstName, string lastName, string mobile,
                                        string ipAddress);

        [OperationContract]
        [WebGet(
            UriTemplate =
                "/mergesocial?session={sessionId}&socialAccountId={socialAccountId}&socialAccountType={socialAccountType}&email={email}"
            ,
            BodyStyle = WebMessageBodyStyle.Bare)]
        bool MergeSocial(string sessionId, string socialAccountId, string socialAccountType, string email);

        [OperationContract]
        [WebGet(
            UriTemplate =
                "/verifymobile?session={sessionId}&authenticationId={authenticationId}&mobileCode={mobileCode}",
            BodyStyle = WebMessageBodyStyle.Bare)]
        bool VerifyMobile(string sessionId, string authenticationId, string mobileCode);

        [OperationContract]
        [WebGet(
            UriTemplate =
                "/verifyemail?session={sessionId}&email={email}&emailcode={emailCode}",
            BodyStyle = WebMessageBodyStyle.Bare)]
        bool VerifyEmail(string sessionId, string email, string emailCode);

        [OperationContract]
        [WebGet(
            UriTemplate =
                "/forgotpasswordbyemail?session={sessionId}&email={email}",
            BodyStyle = WebMessageBodyStyle.Bare)]
        ForgotPasswordResponse ForgotPasswordByEmail(string sessionId, string email);

        [OperationContract]
        [WebGet(
            UriTemplate =
                "/resendmobilecode?session={sessionId}&authenticationId={authenticationId}",
            BodyStyle = WebMessageBodyStyle.Bare)]
        ResendMobileCodeResponse ResendMobileCode(string sessionId, string authenticationId);

        [OperationContract]
        [WebGet(
            UriTemplate =
                "/resendemailverification?session={sessionId}&authenticationId={authenticationId}",
            BodyStyle = WebMessageBodyStyle.Bare)]
        ResendEmailCodeResponse ResendEmailVerification(string sessionId, string authenticationId);

        [OperationContract]
        [WebGet(
            UriTemplate =
                "/validate/{authenticationId}?session={sessionId}",
            BodyStyle = WebMessageBodyStyle.Bare)]
        LoginResponse Validate(string sessionId, string authenticationId);

        [OperationContract]
        [WebGet(
            UriTemplate =
                "/logout/{authenticationId}?session={sessionId}",
            BodyStyle = WebMessageBodyStyle.Bare)]
        bool Logout(string sessionId, string authenticationId);


        [OperationContract]
        [WebGet(
            UriTemplate =
                "/getaccount/{authenticationId}?session={sessionId}",
            BodyStyle = WebMessageBodyStyle.Bare)]
        GetAccountResponse GetAccount(string sessionId, string authenticationId);

        [OperationContract]
        [WebInvoke(
            UriTemplate =
                "/changepassword?session={sessionId}&authId={authenticationId}",
            BodyStyle = WebMessageBodyStyle.Bare)]
        bool ChangePassword(string sessionId, string authenticationId, ChangePasswordRequest request);

        [OperationContract]
        [WebGet(
            UriTemplate =
                "/resetpassword?session={sessionId}&email={email}&emailCode={emailCode}&newPwd={newPassword}",
            BodyStyle = WebMessageBodyStyle.Bare)]
        bool ResetPassword(string sessionId, string email, string emailCode, string newPassword);
    }
}
