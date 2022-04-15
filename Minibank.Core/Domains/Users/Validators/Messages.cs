namespace Minibank.Core.Domains.Users.Validators
{
    public static class Messages
    {
        public static string EmptyLogin = "Login must not be empty!";
        public static string LoginWithSpaces = "Login must not have some spaces!";
        public static string LoginWithLengthMoreThan20 = "Login's length must not be more than 20 symbols!";
        public static string LoginFormat = "Login is not in correct format!";
        public static string EmptyEmail = "Email must not be empty!";
        public static string EmailWithSpaces = "Email must not have some spaces!";
        public static string EmailFormat = "Email is not in correct format!";
        public static string EmailNotMailRu = "Email must be ended on mail.ru!";
    }
}