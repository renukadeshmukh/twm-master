using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TravelWithMe.API.Services.DataContract;

namespace TravelWithMe.API.Services.DataContract
{
    public class LoginRequest 
    {
        public string Email { get; set; }

        public string Password { get; set; }
    }

    public class LoginResponse : BaseResponse
    {
        public Account Account { get; set; }

        public string AuthenticationId { get; set; }
    }

    public class GetAccountResponse : BaseResponse
    {
        public Account UserAccount { get; set; }
    }

    public class ChangeEmailResponse : BaseResponse
    {
    }

    public class ChangePasswordRequest
    {
        public string CurrentPassword { get; set; }

        public string NewPassword { get; set; }
    }

    public class ChangePersonalInfoResponse : BaseResponse
    {
    }

    public class ForgotPasswordResponse : BaseResponse
    {
    }

    public class RegisterResponse : LoginResponse
    {
    }

    public class ResendEmailCodeResponse : BaseResponse
    {
    }

    public class GetBusOperatorInfoResponse : BaseResponse
    {
        public BusOperator BusOperator { get; set; }
    }

    public class SaveBusOperatorResponse : BaseResponse
    {
        public List<FailedObjects> AddressSaveFailures { get; set; }

        public string BankAccountSaveFailure { get; set; }
    }

    public class FailedObjects
    {
        public int Index { get; set; }

        public string ErrorMessage { get; set; }
    }
}
