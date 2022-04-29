using System.Threading.Tasks;

namespace Minibank.Core.Domains.MoneyTransferHistory.Repositories
{
    public interface IMoneyTransferRepository
    {
        public Task AddHistory(double value, string currencyCode, int fromAccountId, int toAccountId);
    }
}