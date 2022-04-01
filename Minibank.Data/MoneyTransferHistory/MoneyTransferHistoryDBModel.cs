namespace Minibank.Data.MoneyTransferHistory
{
    public class MoneyTransferHistoryDBModel
    {
        public int Id { get; set; }
        public double Value { get; set; }
        public string CurrencyCode { get; set; }
        public int FromAccountId { get; set; }
        public int ToAccountId { get; set; }
    }
}