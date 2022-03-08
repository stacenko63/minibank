using System;
using System.Collections.Generic;
using Minibank.Core;

namespace Minibank.Data
{
    public class Database : IDatabase
    {
        private readonly Dictionary<string, int> _currencyDict = new Dictionary<string, int>()
        {
            {"usd",83},
            {"eur",93},
            {"gbp", 113 },
            {"aud", 63}, 
            {"try", 6},
            {"chf", 90},
            {"cad", 68},
            {"nzd", 56},
            {"brl", 16}
        };

        private readonly Random rnd;

        public Database()
        {
            rnd = new Random();
        }
        // public int Get(string currencyCode)
        // {
        //     return _currencyDict.ContainsKey(currencyCode) ? _currencyDict[currencyCode] : -1;
        // }

        public int Get(string currencyCode)
        {
            return _currencyDict.ContainsKey(currencyCode) ? rnd.Next(1,100): -1;
        }
    }
}