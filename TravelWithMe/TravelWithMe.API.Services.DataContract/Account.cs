using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TravelWithMe.API.Core.Model;

namespace TravelWithMe.API.Services.DataContract
{
    public class Account
    {
        public string AccountId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public Address BillingAddress { get; set; }

        public string AccountType { get; set; }

        public bool IsEnabled { get; set; }
    }
}
