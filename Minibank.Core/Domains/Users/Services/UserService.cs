using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Minibank.Core.Domains.BankAccounts.Repositories;
using Minibank.Core.Domains.Users.Repositories;

namespace Minibank.Core.Domains.Users.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        private readonly IBankAccountRepository _bankAccountRepository;

        private readonly IUnitOfWork _unitOfWork;

        private readonly IValidator<User> _userValidator;
        public UserService(IUserRepository userRepository, IBankAccountRepository bankAccountRepository, IUnitOfWork unitOfWork, IValidator<User> userValidator)
        {
            _userRepository = userRepository;
            _bankAccountRepository = bankAccountRepository;
            _unitOfWork = unitOfWork;
            _userValidator = userValidator;
        }
        public async Task<User> GetUser(int id)
        {
            return await _userRepository.GetUser(id);
        }

        public async Task CreateUser(User user)
        {
            await _userValidator.ValidateAndThrowAsync(user);
            await _userRepository.CreateUser(user.Login, user.Email);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<IEnumerable<User>> GetAllUsers()
        {
            return await _userRepository.GetAllUsers();
        }

        public async Task UpdateUser(User user)
        {
            await _userValidator.ValidateAndThrowAsync(user);
            await _userRepository.UpdateUser(user);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteUser(int id)
        {
            if (_bankAccountRepository.HasBankAccounts(id).Result)
            {
                throw new ValidationException("You can't delete user which have one or more BanAccounts");
            }
            await _userRepository.DeleteUser(id);
            await _unitOfWork.SaveChangesAsync();
        }

    }
}