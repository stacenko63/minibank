namespace Minibank.Core.Domains.BankAccounts.Services
{
    public static class Messages
    {
        public static string UpdateUserId = "You can't update bank account's user Id!";
        public static string UpdateCurrency = "You can't update bank account's currency!";
        public static string CloseAccountWithNotZeroBalance = "Before closing BankAccount your balance must be 0!";
        public static string CloseAccountThatAreAlreadyClosed = "This account has already closed!";
        public static string ZeroOrNegativeValue = "Value must be more, than 0!";
        public static string GetCommissionForClosedAccount =
            "You can't get commission, because one of this accounts is closed!";
        public static string MoneyTransferBetweenClosedAccounts =
            "You can't do money transfer, because one of this accounts is closed!";
        public static string TransferMoneyToTheSameAccount = "You can't transfer money to the same account!";
        public static string NotEnoughBalance = "You don't have enough funds on your balance!";
        public static string NonExistentAccount = "Account with this id is not found!";
    }
}