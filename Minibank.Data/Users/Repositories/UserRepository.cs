using System;
using System.Collections.Generic;
using System.Linq;
using Minibank.Core;
using Minibank.Core.Domains.Users;
using Minibank.Core.Domains.Users.Repositories;

namespace Minibank.Data.Users.Repositories
{
    public class UserRepository :IUserRepository
    {
        private static List<UserDBModel> _userDbModels = new List<UserDBModel>();  

        public User GetUser(string id)
        {
            var entity = _userDbModels.FirstOrDefault(i => i.Id == id);
            if (entity == null) throw new ValidationException("User with this id is not found!");
            return new User
            {
                Id = entity.Id,
                Login = entity.Login,
                //IsActive = entity.IsActive
                Email = entity.Email,
                HasBankAccounts = entity.HasBankAccounts
            };
        }

        public void CreateUser(User user)
        {
            var entity = new UserDBModel
            {
                Id = Guid.NewGuid().ToString(),
                Login = user.Login,
                //IsActive = false
                Email = user.Email,
                HasBankAccounts = false
            };
            _userDbModels.Add(entity);
        }

        public IEnumerable<User> GetAllUsers()
        {
            return _userDbModels.Select(it => new User
            {
                Id = it.Id,
                Login = it.Login,
                Email = it.Email,
                HasBankAccounts = it.HasBankAccounts
            });
        }

        public void UpdateUser(User user)
        {
            var entity = _userDbModels.FirstOrDefault(it => it.Id == user.Id);
            if (entity != null)
            {
                entity.Login = user.Login;
                entity.Email = user.Email;
                //entity.IsActive = user.IsActive;
                entity.HasBankAccounts = user.HasBankAccounts;
            }
            
        }

        public void DeleteUser(string id)
        {
            var entity = _userDbModels.FirstOrDefault(it => it.Id == id);
            if (entity == null) throw new ValidationException("User with this id is not in base!");
            if (entity.HasBankAccounts)
                throw new ValidationException("You can't delete user which have one or more BanAccounts");
            _userDbModels.Remove(entity);
        }
    }
}