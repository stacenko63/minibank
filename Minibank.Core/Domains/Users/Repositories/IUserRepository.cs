using System.Collections.Generic;

namespace Minibank.Core.Domains.Users.Repositories
{
    public interface IUserRepository
    {
        User GetUser(string id);
        void CreateUser(string login, string email);
        IEnumerable<User> GetAllUsers(); 
        void UpdateUser(User user);
        void DeleteUser(string id);
    }
}