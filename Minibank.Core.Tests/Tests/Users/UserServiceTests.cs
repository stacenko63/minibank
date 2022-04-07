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
using Minibank.Core.Domains.BankAccounts;
using Minibank.Core.Domains.BankAccounts.Repositories;
using Minibank.Core.Domains.Users;
using Minibank.Core.Domains.Users.Repositories;
using Minibank.Core.Domains.Users.Services;
using Minibank.Core.Domains.Users.Validators;
using Moq;
using Xunit;

namespace Minibank.Core.Tests
{
    public class UserServiceTests
    {
        private readonly IUserService _userService;
        private readonly Mock<IUserRepository> _fakeUserRepository;
        private readonly Mock<IBankAccountRepository> _fakeBankAccountRepository;

        public UserServiceTests()
        {
            _fakeUserRepository = new Mock<IUserRepository>();
            _fakeBankAccountRepository = new Mock<IBankAccountRepository>();
            _userService = new UserService(_fakeUserRepository.Object, _fakeBankAccountRepository.Object,
                new Mock<IUnitOfWork>().Object, new UserValidator(_fakeUserRepository.Object));
        }

        [Fact]
        public async Task CreateAndUpdateUser_SuccessPath_ShouldBeCompleteSuccessfully()
        {
            await _userService.CreateUser(new User {Login = "viktor", Email = "a@mail.ru"});
            _fakeUserRepository.Verify(repository => repository.CreateUser("viktor", "a@mail.ru"), Times.Once);
            var user = new User {Id = 1, Email = "a@mail.ru", Login = "viktor"};
            _fakeUserRepository.Setup(repository => repository.GetUser(1).Result).Returns(user);
            await _userService.UpdateUser(user);
            _fakeUserRepository.Verify(repository => repository.UpdateUser(user), Times.Once);
        }
        
        [Fact]
        public async Task CreateAndUpdateUser_CheckValidatorWork_ShouldThrowValidationException()
        {
            await Assert.ThrowsAsync<FluentValidation.ValidationException>(() =>
                         _userService.CreateUser(new User {Login = "", Email = "a@mail.ru"}));
            await Assert.ThrowsAsync<FluentValidation.ValidationException>(() =>
                _userService.UpdateUser(new User {Login = "", Email = "a@mail.ru"}));
        }

        [Fact]
        public async Task DeleteUser_UserWithBankAccount_ShouldThrowValidationException()
        {
            _fakeBankAccountRepository.Setup(repository => repository.HasBankAccounts(4).Result)
                .Returns(true);
            await Assert.ThrowsAsync<ValidationException>(() =>
                _userService.DeleteUser(4));
        }
        
        [Fact]
        public async Task DeleteUser_UserWithNotInBase_ShouldThrowValidationException()
        {
            _fakeUserRepository.Setup(repository => repository.DeleteUser(4)).Throws<Exception>();
            await Assert.ThrowsAsync<Exception>(() => _userService.DeleteUser(4));
        }
        
        [Fact]
        public async Task DeleteUser_SuccessPath_ShouldDeleteUser()
        {
            _fakeBankAccountRepository.Setup(repository => repository.HasBankAccounts(4).Result)
                .Returns(false);
            await _userService.DeleteUser(4);
            _fakeUserRepository.Verify(repository => repository.DeleteUser(4), Times.Once);
        }

        [Fact]
        public async Task GetUser_UserWithNotInBase_ShouldThrowValidationException()
        {
            _fakeUserRepository.Setup(repository => repository.GetUser(4).Result).Throws<Exception>();
            await Assert.ThrowsAsync<Exception>(() => _userService.GetUser(4));
        }
        
        [Fact]
        public async Task GetUser_SuccessPath_ShouldGetUserFromBase()
        {
            _fakeUserRepository.Setup(repository => repository.GetUser(4).Result).Returns(new 
                User{Id = 4, Email = "a@mail.ru", Login = "viktor"});
            var result = await _userService.GetUser(4);
            Assert.True(result.Id == 4);
            Assert.True(result.Email == "a@mail.ru"); 
            Assert.True(result.Login == "viktor");
        }

        [Fact]
        public async Task GetAllUsers_SuccessPath_ShouldGetAllUsersFromBase()
        {
            _fakeUserRepository.Setup(repository => repository.GetAllUsers().Result).Returns(new
                List<User>
                {
                    new User {Id = 1, Email = "a@mail.ru", Login = "viktor1"},
                });
            var result = await _userService.GetAllUsers();
            var element1 = result.First(it => it.Id == 1);
            Assert.True(result.Count() == 1);
            Assert.True(element1.Email == "a@mail.ru" && element1.Login == "viktor1" );
        }
    }
}