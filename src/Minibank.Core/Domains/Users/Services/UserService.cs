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
            
            if (await _userRepository.ContainsLogin(user.Login))
            {
                throw new ValidationException(Messages.LoginIsAlreadyUsed);
            }
            
            if (await _userRepository.ContainsEmail(user.Email))
            {
                throw new ValidationException(Messages.EmailIsAlreadyUsed);
            }
            
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
            
            if (await _userRepository.ContainsLogin(user.Login))
            {
                throw new ValidationException(Messages.LoginIsAlreadyUsed);
            }
            
            if (await _userRepository.ContainsEmail(user.Email))
            {
                throw new ValidationException(Messages.EmailIsAlreadyUsed);
            }
            
            var updateUser = await _userRepository.GetUser(user.Id);
            
            await _userRepository.UpdateUser(updateUser);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteUser(int id)
        {
            if (await _bankAccountRepository.HasBankAccounts(id))
            {
                throw new ValidationException(Messages.DeleteUserWithBankAccounts);
            }
            
            var user = await _userRepository.GetUser(id);
            
            await _userRepository.DeleteUser(user.Id);
            await _unitOfWork.SaveChangesAsync();
        }

    }
}