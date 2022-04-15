using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Minibank.Core;
using Minibank.Core.Domains.Users;
using Minibank.Core.Domains.Users.Repositories;
using Minibank.Core.Domains.Users.Services;

namespace Minibank.Data.Users.Repositories
{
    public class UserRepository :IUserRepository
    {

        private readonly MiniBankContext _context;
        public UserRepository(MiniBankContext context)
        {
            _context = context;
        }

        public async Task<User> GetUser(int id)
        { 
            var entity = await _context.Users.AsNoTracking().FirstOrDefaultAsync(i => i.Id == id);
           if (entity == null)
           {
               throw new ValidationException(Messages.NonExistentUser);
           }
           return new User
            {
                Id = entity.Id,
                Login = entity.Login,
                Email = entity.Email,
            };
        }

        public async Task CreateUser(string login, string email)
        {
            await _context.Users.AddAsync(new UserDBModel
            {
                Login = login,
                Email = email,
            });
        }

        public async Task<IEnumerable<User>> GetAllUsers()
        {
            await _context.SaveChangesAsync();
            return await _context.Users.AsNoTracking().Select(it => new User
            {
                Id = it.Id,
                Login = it.Login,
                Email = it.Email,
            }).ToArrayAsync();
        }

        public async Task UpdateUser(User user)
        {
            var entity = await _context.Users.FirstAsync(it => it.Id == user.Id);
            entity.Login = user.Login;
            entity.Email = user.Email;
        }

        public async Task DeleteUser(int id)
        {
            var entity = await _context.Users.FirstAsync(it => it.Id == id);
            _context.Users.Remove(entity);
        }

        public async Task<bool> ContainsLogin(string login)
        {
            var entity = await _context.Users.FirstOrDefaultAsync(it => it.Login == login);
            return entity != null; 
        } 
        
        public async Task<bool> ContainsEmail(string email)
        {
            var entity = await _context.Users.FirstOrDefaultAsync(it => it.Email == email);
            return entity != null; 
        } 
    }
}