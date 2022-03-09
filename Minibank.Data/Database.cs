using System;
using System.Collections.Generic;
using Minibank.Core;

namespace Minibank.Data
{
    public class Database : IDatabase
    {
        private readonly Dictionary<string, int> _currencyDict = new Dictionary<string, int>()
        {
            {"usd", 83},
            {"eur", 93},
            {"gbp", 113},
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
        //     if (!_currencyDict.ContainsKey(currencyCode.ToLower())) 
        //         throw new UserFriendlyException("The code of the specified currency was not found in our database!"); 
        //     return _currencyDict[currencyCode.ToLower()];
        // }

        public int Get(string currencyCode)
        {
            if (!_currencyDict.ContainsKey(currencyCode.ToLower()))
                throw new UserFriendlyException("The code of the specified currency was not found in our database!"); 
            return rnd.Next(1,100);
        }
    }
}