namespace Minibank.Core.Domains.MoneyTransferHistory.Services
{
    public interface IMoneyTransferHistoryService
    {
        public void AddHistory(double value, string currencyCode, int fromAccountId, int toAccountId);
    }
}