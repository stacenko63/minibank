using System;
using Minibank.Core.Domains.BankAccounts;

namespace Minibank.Core.Tests.Tests.BankAccounts
{
    public static class ConstValues
    {
        public static int NegativeBalance = -10000;
        public static int CorrectBalance = 100000;
        public static int BankAccountId1 = 1;
        public static int BankAccountId2 = 2;
        public static string CorrectCurrency = "RUB";
        
        public static DateTime OpenDateTime = DateTime.MinValue;
        public static DateTime CloseDateTime = DateTime.MinValue;
        
        public static BankAccount CorrectBankAccount = new BankAccount
        {
            Id = BankAccountId1,
            UserId = Users.ConstValues.UserId1,
            Balance = CorrectBalance,
            Currency = CorrectCurrency, 
            IsOpen = true,
            OpenAccountDate = OpenDateTime,
            CloseAccountDate = CloseDateTime
        };
    }
}