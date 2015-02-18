using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TravelWithMe.API.Core.Model
{
    public class BusOperator
    {
        public int OperatorId { get; set; }

        public string ComapanyName { get; set; }

        public List<string> Email { get; set; }

        public List<string> PhoneNumber { get; set; }

        public List<Address> Addresses { get; set; }

        public BankAccount BankAccount { get; set; }
    }
}
