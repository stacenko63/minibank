namespace Minibank.Core.Domains.BankAccounts.Services
{
    public interface IBankAccountService
    {
        public void CreateBankAccount(string userId, string currencyCode, double startBalance);

        public void CloseAccount(string id);
        
        public double GetCommision(double value, string fromAccountId, string toAccountId);
        
        public void MakeMoneyTransfer(double value, string fromAccountId, string toAccountId);

    }
}