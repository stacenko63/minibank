using System;
using System.Linq;
using Minibank.Core.Enums;

namespace Minibank.Core.Domains.BankAccounts.Validators
{
    public static class Messages
    {
        public static string NegativeStartBalance = "Start balance must be more than 0 or equal 0!";
        public static string IncorrectCurrency = "This currency is incorrect!";
        
        public static string NotPermittedCurrency =
            "This currency is unavailable at the moment! Permitted Currencies: " + 
            string.Join(" ", Enum.GetValues<PermittedCurrencies>().ToArray());
    }
}