using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TravelWithMe.API.Services.DataContract
{
    public class BusOperator
    {
        public string CompanyName { get; set; }

        public List<string> Email { get; set; }

        public List<string> PhoneNumber { get; set; }

        public List<Address> Addresses { get; set; }

        public BankAccount BankAccount { get; set; }
    }
}
