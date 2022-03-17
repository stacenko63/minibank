namespace Minibank.Core.Domains.BankAccounts.Repositories
{
    public interface IBankAccountRepository
    {
        public void CreateBankAccount(string userId, string currency, double startBalance);

        public BankAccount GetAccount(string id);

        public double GetCommision(double value, string fromAccountId, string toAccountId);

        public void MakeMoneyTransfer(double value, string fromAccountId, string toAccountId);
        public void CloseAccount(string id);
    }
}