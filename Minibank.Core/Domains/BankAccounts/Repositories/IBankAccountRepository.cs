namespace Minibank.Core.Domains.BankAccounts.Repositories
{
    public interface IBankAccountRepository
    {
        public void CreateBankAccount(string userId, string currency, double startBalance);

        public BankAccount GetAccount(string id);
        
        public void CloseAccount(string id);

        public bool HasBankAccounts(string userId);

        public void MakeMoneyTransfer(double valueFrom, double valueTo, string fromAccountId, string toAccountId);
    }
}