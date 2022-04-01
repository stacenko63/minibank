using System;

namespace Minibank.Data.BankAccounts
{
    public class BankAccountDBModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public double Balance { get; set; }
        public string Currency { get; set; }
        public bool IsOpen { get; set; }
        public DateTime OpenAccountDate { get; set; }
        public DateTime CloseAccountDate { get; set; }
    }
}