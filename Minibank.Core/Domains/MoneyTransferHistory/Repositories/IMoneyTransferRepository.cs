namespace Minibank.Core.Domains.MoneyTransferHistory.Repositories
{
    public interface IMoneyTransferRepository
    {
        public void AddHistory(double value, string currencyCode, string fromAccountId, string toAccountId);
    }
}