using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Minibank.Data.MoneyTransferHistory
{
    [Table("money_transfer_history")]
    public class MoneyTransferHistoryDBModel
    {
        [Column("id")]
        public int Id { get; set; }
        [Column("value")]
        public double Value { get; set; }
        [Column("currency_code")]
        public string CurrencyCode { get; set; }
        [Column("from_account_id")]
        public int FromAccountId { get; set; }
        [Column("to_account_id")]
        public int ToAccountId { get; set; }
    }
}