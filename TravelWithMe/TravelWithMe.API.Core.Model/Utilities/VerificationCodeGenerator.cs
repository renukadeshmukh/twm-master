using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TravelWithMe.API.Core.Model.Utilities
{
    public static class VerificationCodeGenerator
    {
        private static readonly Random Random = new Random();

        public static string GenerateNewVerificationCode()
        {
            int random = Random.Next(100000, 999999);

            return random.ToString();
        }
    }
}
