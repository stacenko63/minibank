using System;

namespace Minibank.Core.Domains.BankAccounts
{
    public class BankAccount
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public double Balance { get; set; }
        public string Currency { get; set; }
        public bool IsOpen { get; set; }
        public DateTime OpenAccountDate { get; set; }
        public DateTime CloseAccountDate { get; set; }
        
    }
}