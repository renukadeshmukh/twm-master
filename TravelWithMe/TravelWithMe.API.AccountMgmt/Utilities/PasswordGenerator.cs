using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TravelWithMe.API.AccountMgmt.Utilities
{
    public static class PasswordGenerator
    {
        private static readonly Random Random = new Random();

        public static string GenerateRandomPassword()
        {
            var stringBuilder = new StringBuilder();
            for (int count = 1; count <= 8; count++)
            {
                stringBuilder.Append((char)Random.Next('A', 'Z'));
            }
            return stringBuilder.ToString();
        }
    }
}
