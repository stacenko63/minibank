using System.Collections.Generic;
using System.Threading.Tasks;

namespace Minibank.Core.Domains.Users.Services
{
    public interface IUserService
    {
        Task<User> GetUser(int id);
        Task CreateUser(User user);
        Task<IEnumerable<User>> GetAllUsers(); 
        Task UpdateUser(User user);
        Task DeleteUser(int id);
        
    }
}