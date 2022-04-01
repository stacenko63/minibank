using System.Collections.Generic;
using System.Threading.Tasks;

namespace Minibank.Core.Domains.Users.Repositories
{
    public interface IUserRepository
    {
        Task<User> GetUser(int id);
        Task CreateUser(string login, string email);
        Task<IEnumerable<User>> GetAllUsers(); 
        Task UpdateUser(User user);
        Task DeleteUser(int id);
    }
}