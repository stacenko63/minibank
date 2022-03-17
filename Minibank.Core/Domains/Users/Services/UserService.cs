using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Minibank.Core.Domains.Users.Repositories;

namespace Minibank.Core.Domains.Users.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        
        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public User GetUser(string id)
        {
            return _userRepository.GetUser(id);
        }

        public void CreateUser(User user)
        {
            if (string.IsNullOrEmpty(user.Login)) throw new ValidationException("Incorrect login!");
            if (user.Login.Contains(" ")) throw new ValidationException("Login must not have some spaces!");
            if (user.Login.Length > 20)
                throw new ValidationException("Login's length must not be more that 20 symbols!");
            _userRepository.CreateUser(user);
        }

        public IEnumerable<User> GetAllUsers()
        {
            return _userRepository.GetAllUsers()
                .Select(it => new User
                {
                    Id = it.Id,
                    Login = it.Login,
                    Email = it.Email,
                    HasBankAccounts = it.HasBankAccounts
                });
        }

        public void UpdateUser(User user)
        {
            _userRepository.UpdateUser(user);
        }

        public void DeleteUser(string id)
        {
            _userRepository.DeleteUser(id);
        }
        
        

        // public void SetActive(string id)
        // {
        //     var user = _userRepository.Get(id);
        //     if (user == null) return;
        //     if (user.IsActive) throw new ValidationException("User has already active!");
        //     user.IsActive = true;
        //     _userRepository.Update(user);
        // }
    }
}