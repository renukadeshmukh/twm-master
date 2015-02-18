using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TravelWithMe.Test.TestDataProvider
{
    public class RandomGenerator
    {
        private static readonly Random Random = new Random();

        internal static string GenerateRandomNumberString(int digits = 1, int minValue = 0, int maxValue = 9,
                                                          int multiplesOf = 1)
        {
            var stringBuilder = new StringBuilder();
            for (int count = 1; count <= digits; count += multiplesOf)
            {
                stringBuilder.Append(Random.Next(minValue, maxValue));
            }
            return stringBuilder.ToString();
        }
    }
}
