using System.Threading.Tasks;

namespace Minibank.Core.Domains.BankAccounts.Services
{
    public interface IBankAccountService
    {
        public Task CreateBankAccount(int userId, string currencyCode, double startBalance);

        public Task CloseAccount(int id);
        
        public Task<double> GetCommission(double value, int fromAccountId, int toAccountId);
        
        public Task MakeMoneyTransfer(double value, int fromAccountId, int toAccountId);
        public Task UpdateBankAccount(BankAccount bankAccount);

        public Task<BankAccount> GetAccount(int id);

    }
}