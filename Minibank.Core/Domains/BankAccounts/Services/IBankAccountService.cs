namespace Minibank.Core.Domains.BankAccounts.Services
{
    public interface IBankAccountService
    {
        public void CreateBankAccount(int userId, string currencyCode, double startBalance);

        public void CloseAccount(int id);
        
        public double GetCommision(double value, int fromAccountId, int toAccountId);
        
        public void MakeMoneyTransfer(double value, int fromAccountId, int toAccountId);

    }
}