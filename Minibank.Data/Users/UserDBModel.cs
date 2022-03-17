namespace Minibank.Data.Users
{
    public class UserDBModel
    {
        public string Id { get; set; }
        public string Login { get; set; }
        //public bool IsActive { get; set; }
        public string Email { get; set; }
        public bool HasBankAccounts { get; set; }
        
        
    }
}