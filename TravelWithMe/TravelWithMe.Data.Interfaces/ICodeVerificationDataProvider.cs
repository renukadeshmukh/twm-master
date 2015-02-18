using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TravelWithMe.Data.Interfaces
{
    public interface ICodeVerificationDataProvider
    {
        void SaveNewCode(string key, string value, string type);

        bool IsValid(string key, string value, string type);

        bool Remove(string key, string type);
    }
}
