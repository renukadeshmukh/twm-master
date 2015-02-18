using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TravelWithMe.API.Core.Model
{
    public class BankAccount
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public string BankName { get; set; }

        public string IFSCCode { get; set; }

        public string Branch { get; set; }

        public string AccountHolderName { get; set; }

        public string AccountNumber { get; set; }
    }
}
