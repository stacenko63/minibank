using Minibank.Core.Domains.Users;

namespace Minibank.Core.Tests.Tests.Users
{
    public static class ConstValues
    {
        public static string CorrectEmail = "a@mail.ru";
        public static string CorrectLogin = "viktor";
        public static int UserId1 = 1;
        public static int UserId2 = 2;

        public static User CorrectUser = new User
        {
            Id = UserId1,
            Login = CorrectLogin,
            Email = CorrectEmail
        };
    }
}