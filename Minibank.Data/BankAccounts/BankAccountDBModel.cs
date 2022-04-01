using System;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Minibank.Data.BankAccounts
{
    [Table("bank_account")]
    public class BankAccountDBModel
    {
        [Column("id")]
        public int Id { get; set; }
        [Column("user_id")]
        public int UserId { get; set; }
        [Column("balance")]
        public double Balance { get; set; }
        [Column("currency")]
        public string Currency { get; set; }
        [Column("is_open")]
        public bool IsOpen { get; set; }
        [Column("open_account_date")]
        public DateTime OpenAccountDate { get; set; }
        [Column("close_account_date")]
        public DateTime CloseAccountDate { get; set; }
    }
}