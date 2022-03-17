using System.Collections.Generic;

namespace Minibank.Core.Domains.Users.Repositories
{
    public interface IUserRepository
    {
        User GetUser(string id);
        void CreateUser(User user);
        IEnumerable<User> GetAllUsers(); 
        void UpdateUser(User user);
        void DeleteUser(string id);
    }
}