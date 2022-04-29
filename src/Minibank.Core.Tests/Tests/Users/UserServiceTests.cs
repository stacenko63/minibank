using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.AspNetCore.SignalR.Protocol;
using Minibank.Core.Domains.BankAccounts;
using Minibank.Core.Domains.BankAccounts.Repositories;
using Minibank.Core.Domains.Users;
using Minibank.Core.Domains.Users.Repositories;
using Minibank.Core.Domains.Users.Services;
using Minibank.Core.Domains.Users.Validators;
using Minibank.Core.Tests.Tests.Users;
using Moq;
using Xunit;
using Messages = Minibank.Core.Domains.Users.Services.Messages;

namespace Minibank.Core.Tests
{
    public class UserServiceTests
    {
        private readonly IUserService _userService;
        private readonly Mock<IUserRepository> _fakeUserRepository;
        private readonly Mock<IBankAccountRepository> _fakeBankAccountRepository;
        private readonly UserValidate _userValidator;  

        public UserServiceTests()
        {
            _userValidator = new UserValidate();
            _fakeUserRepository = new Mock<IUserRepository>();
            _fakeBankAccountRepository = new Mock<IBankAccountRepository>();
            _userService = new UserService(_fakeUserRepository.Object, _fakeBankAccountRepository.Object,
                new Mock<IUnitOfWork>().Object, _userValidator);
        }

        [Fact]
        public async Task CreateUser_LoginWhichHasAlreadyUsed_ShouldThrowValidationException()
        {
            _fakeUserRepository.Setup(repository => repository.ContainsLogin(ConstValues.CorrectLogin)).ReturnsAsync(true);
            
            var exception = await Assert.ThrowsAsync<ValidationException>(() =>
                _userService.CreateUser(ConstValues.CorrectUser));
            Assert.Equal(Messages.LoginIsAlreadyUsed, exception.Message);
        }
        
        [Fact]
        public async Task CreateUser_EmailWhichHasAlreadyUsed_ShouldThrowValidationException()
        {
            _fakeUserRepository.Setup(repository => repository.ContainsEmail(ConstValues.CorrectEmail)).ReturnsAsync(true);
            
            var exception = await Assert.ThrowsAsync<ValidationException>(() =>
                _userService.CreateUser(ConstValues.CorrectUser));
            Assert.Equal(Messages.EmailIsAlreadyUsed, exception.Message);
        }
        
        [Fact]
        public async Task CreateUser_SuccessPath_ShouldCallRepository()
        {
            await _userService.CreateUser(ConstValues.CorrectUser);
            
            _fakeUserRepository.Verify(repository => repository.
                CreateUser(ConstValues.CorrectLogin, ConstValues.CorrectEmail), Times.Once);
        }
        
        [Fact]
        public async Task CreateUser_CheckValidatorWork_ShouldCallValidator()
        {
            await _userService.CreateUser(ConstValues.CorrectUser);
            
            Assert.True(_userValidator.IsCalled);
        }
        
        [Fact]
        public async Task UpdateUser_LoginWhichHasAlreadyUsed_ShouldThrowValidationException()
        {
            _fakeUserRepository.Setup(repository => repository.ContainsLogin(ConstValues.CorrectLogin)).ReturnsAsync(true);
            
            var exception = await Assert.ThrowsAsync<ValidationException>(() =>
                _userService.UpdateUser(ConstValues.CorrectUser));
            Assert.Equal(Messages.LoginIsAlreadyUsed, exception.Message);
        }
        
        [Fact]
        public async Task UpdateUser_EmailWhichHasAlreadyUsed_ShouldThrowValidationException()
        {
            _fakeUserRepository.Setup(repository => repository.ContainsEmail(ConstValues.CorrectEmail)).ReturnsAsync(true);
            
            var exception = await Assert.ThrowsAsync<ValidationException>(() =>
                _userService.UpdateUser(ConstValues.CorrectUser));
            Assert.Equal(Messages.EmailIsAlreadyUsed, exception.Message);
        }

        [Fact]
        public async Task UpdateUser_SuccessPath_ShouldCallRepository()
        {

            _fakeUserRepository.Setup(repository => repository.
                GetUser(ConstValues.UserId1)).ReturnsAsync(ConstValues.CorrectUser);
            await _userService.UpdateUser(ConstValues.CorrectUser);
            
            _fakeUserRepository.Verify(repository => repository.UpdateUser(ConstValues.CorrectUser), Times.Once);
        }

        [Fact]
        public async Task UpdateUser_CheckValidatorWork_ShouldCallValidator()
        {
            await _userService.UpdateUser(ConstValues.CorrectUser);
            
            Assert.True(_userValidator.IsCalled);
        }
        

        [Fact]
        public async Task DeleteUser_UserWithBankAccount_ShouldThrowValidationException()
        {
            _fakeBankAccountRepository.Setup(repository => repository.HasBankAccounts(ConstValues.UserId1))
                .ReturnsAsync(true);
            
            var exception = await Assert.ThrowsAsync<ValidationException>(() =>
                _userService.DeleteUser(ConstValues.UserId1));
            Assert.Equal(Messages.DeleteUserWithBankAccounts, exception.Message);
        }
        
        [Fact]
        public async Task DeleteUser_NonExistentUser_ShouldThrowValidationException()
        {

            _fakeUserRepository.Setup(repository => repository.GetUser(ConstValues.UserId1)).
                Throws(new ValidationException(Messages.NonExistentUser));
            
            var exception = await Assert.ThrowsAsync<ValidationException>(() => _userService.DeleteUser(ConstValues.UserId1));
            Assert.Equal(Messages.NonExistentUser, exception.Message);
        }
        
        [Fact]
        public async Task DeleteUser_SuccessPath_ShouldDeleteUser()
        {
            _fakeBankAccountRepository.Setup(repository => repository.HasBankAccounts(ConstValues.UserId1))
                .ReturnsAsync(false);
            _fakeUserRepository.Setup(repository => repository.GetUser(ConstValues.UserId1))
                .ReturnsAsync(ConstValues.CorrectUser);
            
            await _userService.DeleteUser(ConstValues.UserId1);
            
            _fakeUserRepository.Verify(repository => repository.DeleteUser(ConstValues.UserId1), Times.Once);
        }

        [Fact]
        public async Task GetUser_NonExistentUser_ShouldThrowValidationException()
        {
            _fakeUserRepository.Setup(repository => repository.GetUser(ConstValues.UserId1)).
                ThrowsAsync(new ValidationException(Messages.NonExistentUser));
            
            var exception = await Assert.ThrowsAsync<ValidationException>(() => _userService.GetUser(ConstValues.UserId1));
            Assert.Equal(Messages.NonExistentUser, exception.Message);
        }
        
        [Fact]
        public async Task GetUser_SuccessPath_ShouldGetUserFromBase()
        {
            _fakeUserRepository.Setup(repository => repository.GetUser(ConstValues.UserId1)).ReturnsAsync(ConstValues.CorrectUser);
            var result = await _userService.GetUser(ConstValues.UserId1);
            
            Assert.True(result.Id == ConstValues.UserId1);
            Assert.True(result.Email == ConstValues.CorrectEmail); 
            Assert.True(result.Login == ConstValues.CorrectLogin);
        }

        [Fact]
        public async Task GetAllUsers_SuccessPath_ShouldGetAllUsersFromBase()
        {
            _fakeUserRepository.Setup(repository => repository.GetAllUsers()).ReturnsAsync(new
                List<User>
                {
                    ConstValues.CorrectUser,
                });
            var result = await _userService.GetAllUsers();
            var element1 = result.First(it => it.Id == ConstValues.UserId1);
            
            Assert.True(result.Count() == 1);
            Assert.True(element1.Email == ConstValues.CorrectEmail && element1.Login == ConstValues.CorrectLogin);
        }
        
        class UserValidate : IValidator<User>
        {
            public bool IsCalled { get; set; }

            public UserValidate()
            {
                IsCalled = false;
            }
            public ValidationResult Validate(IValidationContext context)
            {
                IsCalled = true;
                return new ValidationResult();
            }

            public async Task<ValidationResult> ValidateAsync(IValidationContext context, CancellationToken cancellation = new CancellationToken())
            {
                IsCalled = true;
                return new ValidationResult();
            }

            public IValidatorDescriptor CreateDescriptor()
            {
                return new ValidatorDescriptor<User>(new List<IValidationRule>());
            }

            public bool CanValidateInstancesOfType(Type type)
            {
                return true;
            }

            public ValidationResult Validate(User instance)
            {
                IsCalled = true;
                return new ValidationResult();

            }

            public async Task<ValidationResult> ValidateAsync(User instance, CancellationToken cancellation = new CancellationToken())
            {
                IsCalled = true;
                return new ValidationResult();
            }
        }
    }
}