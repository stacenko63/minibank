namespace Minibank.Core.Domains.Users.Services
{
    public static class Messages
    {
        public static string LoginIsAlreadyUsed = "This login is already used!";
        public static string EmailIsAlreadyUsed = "This email is already used!";
        
        public static string DeleteUserWithBankAccounts = "You can't delete user which have one or more BankAccounts";
        public static string NonExistentUser = "User with this id is not found!";
    }
}