using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TravelWithMe.API.Core.Model
{
    public class Account
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string AccountId { get; set; }

        public string PhoneNumber { get; set; }

        public bool IsEnabled { get; set; }

        public AccountType AccountType { get; set; }
    }

    public enum AccountType
    {
        Administrator,
        EndUser,
        BusOperator
    }
}
