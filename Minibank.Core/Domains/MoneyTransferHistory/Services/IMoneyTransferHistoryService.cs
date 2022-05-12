using System.Threading.Tasks;

namespace Minibank.Core.Domains.MoneyTransferHistory.Services
{
    public interface IMoneyTransferHistoryService
    {
        public Task AddHistory(double value, string currencyCode, int fromAccountId, int toAccountId);
    }
}