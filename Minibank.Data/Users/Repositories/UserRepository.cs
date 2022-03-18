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
                Email = entity.Email,
            };
        }

        public void CreateUser(string login, string email)
        {
            _userDbModels.Add(new UserDBModel
            {
                Id = Guid.NewGuid().ToString(),
                Login = login,
                Email = email,
            });
        }

        public IEnumerable<User> GetAllUsers()
        {
            return _userDbModels.Select(it => new User
            {
                Id = it.Id,
                Login = it.Login,
                Email = it.Email,
            });
        }

        public void UpdateUser(User user)
        {
            var entity = _userDbModels.FirstOrDefault(it => it.Id == user.Id);
            if (entity == null)
                throw new ValidationException("You can't update this user, because this id is not found in base!");
            entity.Login = user.Login;
            entity.Email = user.Email;
        }

        public void DeleteUser(string id)
        {
            var entity = _userDbModels.FirstOrDefault(it => it.Id == id);
            if (entity == null) throw new ValidationException("User with this id is not in base!");
            _userDbModels.Remove(entity);
        }
    }
}