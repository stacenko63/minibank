namespace Minibank.Core.Domains.BankAccounts.Repositories
{
    public interface IBankAccountRepository
    {
        public void CreateBankAccount(int userId, string currency, double startBalance);

        public BankAccount GetAccount(int id);
        
        public void CloseAccount(int id);

        public bool HasBankAccounts(int userId);

        public void MakeMoneyTransfer(double valueFrom, double valueTo, int fromAccountId, int toAccountId);
    }
}