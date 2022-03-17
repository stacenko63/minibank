namespace Minibank.Core.Domains.Users
{
    public class User
    {
        public string Id { get; set; }
        public string Login { get; set; }
        // public string Email { get; set; }
        // public bool isActual { get; set; }
        //public bool IsActive { get; set; }
        public string Email { get; set; } 
        
        public bool HasBankAccounts { get; set; }
    }
}