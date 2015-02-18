using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TravelWithMe.API.Core.Model;

namespace TravelWithMe.API.Interfaces
{
    public interface IEmailProvider
    {
        void SendRegistrationEmail(Account account, string password);

        void SendForgotPasswordEmail(string toEmail, string firstName, string lastName, string verificationCode);

        void SendWLEmail(string email, string requestId, string friendlyRequestId, DateTime travelDate,
                         string fromCity, string toCity, List<Passenger> passengers);

        bool VerifyEmail(Account account, string emailCode);

        void ResendEmailVerification(Account account);

        void SendWLResultsEmail(string email, string requestId, DateTime travelDate,
                                string fromCity, string toCity, List<Passenger> passengers);
    }
}
