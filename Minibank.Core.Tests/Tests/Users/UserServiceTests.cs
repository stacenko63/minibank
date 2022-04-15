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
            const string email = "a@mail.ru", login = "Viktor";
            _fakeUserRepository.Setup(repository => repository.ContainsLogin(login).Result).Returns(true);
            var exception = await Assert.ThrowsAsync<ValidationException>(() =>
                _userService.CreateUser(new User {Login = login, Email = email}));
            Assert.Equal(Messages.LoginIsAlreadyUsed, exception.Message);
        }
        
        [Fact]
        public async Task CreateUser_EmailWhichHasAlreadyUsed_ShouldThrowValidationException()
        {
            const string email = "a@mail.ru", login = "Viktor";
            _fakeUserRepository.Setup(repository => repository.ContainsEmail(email).Result).Returns(true);
            var exception = await Assert.ThrowsAsync<ValidationException>(() =>
                _userService.CreateUser(new User {Login = login, Email = email}));
            Assert.Equal(Messages.EmailIsAlreadyUsed, exception.Message);
        }
        
        [Fact]
        public async Task CreateUser_SuccessPath_ShouldBeCompleteSuccessfully()
        {
            const string email = "a@mail.ru", login = "Viktor";
            await _userService.CreateUser(new User {Login = login, Email = email});
            _fakeUserRepository.Verify(repository => repository.CreateUser(login, email), Times.Once);
        }
        
        [Fact]
        public async Task CreateUser_CheckValidatorWork_ShouldCheckValidatorCall()
        {
            await _userService.CreateUser(new User {Login = "viktor", Email = "a@mail.ru"});
            Assert.True(_userValidator.IsCalled);
        }
        
        [Fact]
        public async Task UpdateUser_LoginWhichHasAlreadyUsed_ShouldThrowValidationException()
        {
            const string email = "a@mail.ru", login = "Viktor";
            _fakeUserRepository.Setup(repository => repository.ContainsLogin(login).Result).Returns(true);
            var exception = await Assert.ThrowsAsync<ValidationException>(() =>
                _userService.UpdateUser(new User {Login = login, Email = email}));
            Assert.Equal(Messages.LoginIsAlreadyUsed, exception.Message);
        }
        
        [Fact]
        public async Task UpdateUser_EmailWhichHasAlreadyUsed_ShouldThrowValidationException()
        {
            const string email = "a@mail.ru", login = "Viktor";
            _fakeUserRepository.Setup(repository => repository.ContainsEmail(email).Result).Returns(true);
            var exception = await Assert.ThrowsAsync<ValidationException>(() =>
                _userService.UpdateUser(new User {Login = login, Email = email}));
            Assert.Equal(Messages.EmailIsAlreadyUsed, exception.Message);
        }

        [Fact]
        public async Task UpdateUser_SuccessPath_ShouldBeCompleteSuccessfully()
        {
            const string email = "a@mail.ru", login = "Viktor";
            const int id = 1;
            var user = new User {Id = id, Email = email, Login = login};
            _fakeUserRepository.Setup(repository => repository.GetUser(id).Result).Returns(user);
            await _userService.UpdateUser(user);
            _fakeUserRepository.Verify(repository => repository.UpdateUser(user), Times.Once);
        }

        [Fact]
        public async Task UpdateUser_CheckValidatorWork_ShouldCheckValidatorCall()
        {
            await _userService.UpdateUser(new User {Login = "viktor", Email = "a@mail.ru"});
            Assert.True(_userValidator.IsCalled);
        }
        

        [Fact]
        public async Task DeleteUser_UserWithBankAccount_ShouldThrowValidationException()
        {
            const int id = 4; 
            _fakeBankAccountRepository.Setup(repository => repository.HasBankAccounts(id).Result)
                .Returns(true);
            var exception = await Assert.ThrowsAsync<ValidationException>(() =>
                _userService.DeleteUser(id));
            Assert.Equal(Messages.DeleteUserWithBankAccounts, exception.Message);
        }
        
        [Fact]
        public async Task DeleteUser_NonExistentUser_ShouldThrowValidationException()
        {
            const int id = 4; 
            _fakeUserRepository.Setup(repository => repository.GetUser(id)).Throws(new ValidationException(Messages.NonExistentUser));
            var exception = await Assert.ThrowsAsync<ValidationException>(() => _userService.DeleteUser(id));
            Assert.Equal(Messages.NonExistentUser, exception.Message);
        }
        
        [Fact]
        public async Task DeleteUser_SuccessPath_ShouldDeleteUser()
        {
            const int id = 4; 
            _fakeBankAccountRepository.Setup(repository => repository.HasBankAccounts(id).Result)
                .Returns(false);
            _fakeUserRepository.Setup(repository => repository.GetUser(id).Result).Returns(new User{Id = id});
            await _userService.DeleteUser(id);
            _fakeUserRepository.Verify(repository => repository.DeleteUser(id), Times.Once);
        }

        [Fact]
        public async Task GetUser_NonExistentUser_ShouldThrowValidationException()
        {
            const int id = 4; 
            _fakeUserRepository.Setup(repository => repository.GetUser(id).Result).
                Throws(new ValidationException(Messages.NonExistentUser));
            var exception = await Assert.ThrowsAsync<ValidationException>(() => _userService.GetUser(id));
            Assert.Equal(Messages.NonExistentUser, exception.Message);
        }
        
        [Fact]
        public async Task GetUser_SuccessPath_ShouldGetUserFromBase()
        {
            const string email = "a@mail.ru", login = "viktor";
            const int id = 4;
            _fakeUserRepository.Setup(repository => repository.GetUser(4).Result).Returns(new 
                User{Id = id, Email = email, Login = login});
            var result = await _userService.GetUser(id);
            Assert.True(result.Id == id);
            Assert.True(result.Email == email); 
            Assert.True(result.Login == login);
        }

        [Fact]
        public async Task GetAllUsers_SuccessPath_ShouldGetAllUsersFromBase()
        {
            const string email = "a@mail.ru", login = "viktor1";
            const int id = 1;
            _fakeUserRepository.Setup(repository => repository.GetAllUsers().Result).Returns(new
                List<User>
                {
                    new User {Id = id, Email = email, Login = login},
                });
            var result = await _userService.GetAllUsers();
            var element1 = result.First(it => it.Id == id);
            Assert.True(result.Count() == 1);
            Assert.True(element1.Email == email && element1.Login == login);
        }
    }
}