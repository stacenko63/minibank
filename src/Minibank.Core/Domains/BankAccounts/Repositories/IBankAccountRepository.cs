using System.Threading.Tasks;

namespace Minibank.Core.Domains.BankAccounts.Repositories
{
    public interface IBankAccountRepository
    {
        public Task CreateBankAccount(int userId, string currency, double startBalance);

        public Task<BankAccount> GetAccount(int id);

        public Task<bool> HasBankAccounts(int userId);

        public Task UpdateBankAccount(BankAccount bankAccount);
    }
}