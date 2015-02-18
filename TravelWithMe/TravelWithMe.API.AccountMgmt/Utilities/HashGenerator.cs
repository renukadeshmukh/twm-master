using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace TravelWithMe.API.AccountMgmt.Utilities
{
    public static class HashGenerator
    {
        public static string GenerateHash(string password)
        {
            using (MD5 hasher = MD5.Create())
            {
                byte[] hash = hasher.ComputeHash(Encoding.UTF8.GetBytes(password));
                // Create a new Stringbuilder to collect the bytes
                // and create a string.
                var sBuilder = new StringBuilder();

                // Loop through each byte of the hashed data 
                // and format each one as a hexadecimal string.
                for (int i = 0; i < hash.Length; i++)
                {
                    sBuilder.Append(hash[i].ToString("x2"));
                }

                // Return the hexadecimal string.
                return sBuilder.ToString();
            }
        }
    }
}
