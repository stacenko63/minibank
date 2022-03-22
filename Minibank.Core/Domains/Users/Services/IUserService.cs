using System.Collections.Generic;

namespace Minibank.Core.Domains.Users.Services
{
    public interface IUserService
    {
        User GetUser(int id);
        void CreateUser(User user);
        IEnumerable<User> GetAllUsers(); 
        void UpdateUser(User user);
        void DeleteUser(int id);
    }
}