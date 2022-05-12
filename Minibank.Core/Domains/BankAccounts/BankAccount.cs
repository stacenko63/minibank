using System;

namespace Minibank.Core.Domains.BankAccounts
{
    public class BankAccount
    {
        
        public int Id { get; set; }
        public int UserId { get; set; }
        public double Balance { get; set; }
        public string Currency { get; set; }
        public bool IsOpen { get; set; }
        public DateTime OpenAccountDate { get; set; }
        public DateTime? CloseAccountDate { get; set; }
        
    }
}