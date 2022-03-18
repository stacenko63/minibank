using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Minibank.Core.Domains.BankAccounts.Repositories;
using Minibank.Core.Domains.Users.Repositories;

namespace Minibank.Core.Domains.Users.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        private readonly IBankAccountRepository _bankAccountRepository;
        public UserService(IUserRepository userRepository, IBankAccountRepository bankAccountRepository)
        {
            _userRepository = userRepository;
            _bankAccountRepository = bankAccountRepository;
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
            _userRepository.CreateUser(user.Login, user.Email);
        }

        public IEnumerable<User> GetAllUsers()
        {
            return _userRepository.GetAllUsers();
        }

        public void UpdateUser(User user)
        {
            _userRepository.UpdateUser(user);
        }

        public void DeleteUser(string id)
        {
            if (_bankAccountRepository.HasBankAccounts(id))
                throw new ValidationException("You can't delete user which have one or more BanAccounts");
            _userRepository.DeleteUser(id);
        }

    }
}