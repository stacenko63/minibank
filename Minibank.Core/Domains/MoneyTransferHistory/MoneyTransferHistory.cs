namespace Minibank.Core.Domains.MoneyTransferHistory
{
    public class MoneyTransferHistory
    {
        public string Id { get; set; }
        public double Value { get; set; }
        public string CurrencyCode { get; set; }
        public string FromAccountId { get; set; }
        public string ToAccountId { get; set; }
    }
}