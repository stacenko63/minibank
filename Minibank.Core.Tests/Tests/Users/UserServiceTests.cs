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
        private readonly IValidator<User> _userValidator;
        private readonly Mock<IUserService> _fakeUserServise;
        private readonly Mock<IValidator<User>> _fakeUserValidator;

        public UserServiceTests()
        {
            _fakeUserServise = new Mock<IUserService>();
            
            _fakeUserRepository = new Mock<IUserRepository>();
            _fakeBankAccountRepository = new Mock<IBankAccountRepository>();
            _userValidator = new UserValidator(_fakeUserRepository.Object);
            _fakeUserValidator = new Mock<IValidator<User>>();
            _userService = new UserService(_fakeUserRepository.Object, _fakeBankAccountRepository.Object,
                new Mock<IUnitOfWork>().Object, _userValidator);
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
        public void CreateAndUpdateUser_WithNullLogin_ShouldThrowValidationException()
        {
            Assert.True(!_userService.CreateUser(new User{Login = null, Email = "a@mail.ru"}).IsCompletedSuccessfully);
            Assert.True(!_userService.UpdateUser(new User{Login = null, Email = "a@mail.ru"}).IsCompletedSuccessfully);
        }
        

        // [Fact]
        // public void CreateAndUpdateUser_WithEmptyLogin_ShouldThrowValidationException()
        // {
        //     Assert.ThrowsAsync<ValidationException>(() =>
        //         _userService.CreateUser(new User {Login = "", Email = "a@mail.ru"}));
        //     Assert.ThrowsAsync<ValidationException>(() =>
        //         _userService.UpdateUser(new User {Login = "", Email = "a@mail.ru"}));
        // }
        //
        // [Fact]
        // public void CreateAndUpdateUser_WithLoginWithSpaces_ShouldThrowValidationException()
        // {
        //     Assert.ThrowsAsync<ValidationException>(() =>
        //         _userService.CreateUser(new User {Login = "  ", Email = "a@mail.ru"}));
        //     Assert.ThrowsAsync<ValidationException>(() =>
        //         _userService.CreateUser(new User {Login = " viktor", Email = "a@mail.ru"}));
        //     Assert.ThrowsAsync<ValidationException>(() =>
        //         _userService.CreateUser(new User {Login = "viktor ", Email = "a@mail.ru"}));
        //     Assert.ThrowsAsync<ValidationException>(() =>
        //         _userService.CreateUser(new User {Login = " viktor ", Email = "a@mail.ru"}));
        //     Assert.ThrowsAsync<ValidationException>(() =>
        //         _userService.CreateUser(new User {Login = "s s", Email = "a@mail.ru"}));
        //     
        //     Assert.ThrowsAsync<ValidationException>(() =>
        //         _userService.UpdateUser(new User {Login = "  ", Email = "a@mail.ru"}));
        //     Assert.ThrowsAsync<ValidationException>(() =>
        //         _userService.UpdateUser(new User {Login = " viktor", Email = "a@mail.ru"}));
        //     Assert.ThrowsAsync<ValidationException>(() =>
        //         _userService.UpdateUser(new User {Login = "viktor ", Email = "a@mail.ru"}));
        //     Assert.ThrowsAsync<ValidationException>(() =>
        //         _userService.UpdateUser(new User {Login = " viktor ", Email = "a@mail.ru"}));
        //     Assert.ThrowsAsync<ValidationException>(() =>
        //         _userService.UpdateUser(new User {Login = "s s", Email = "a@mail.ru"}));
        // }
        //
        // [Fact]
        // public void CreateAndUpdateUser_WithLoginWithLengthMoreThan20_ShouldThrowValidationException()
        // {
        //     Assert.ThrowsAsync<ValidationException>(() =>
        //         _userService.CreateUser(new User
        //             {Login = "dddddddddddddddddddddddddddddddddddddddddddddddddddd", Email = "a@mail.ru"}));
        //     Assert.ThrowsAsync<ValidationException>(() =>
        //         _userService.UpdateUser(new User
        //             {Login = "dddddddddddddddddddddddddddddddddddddddddddddddddddd", Email = "a@mail.ru"}));
        // }
        //
        // [Theory]
        // [InlineData(0, 48)]
        // [InlineData(58, 65)]
        // [InlineData(91, 97)]
        // [InlineData(123, 256)]
        // public void CreateAndUpdateUser_WithLoginWithIncorrectFormat_ShouldThrowValidationException(int begin, int end)
        // {
        //     for (int i = begin; i < end; i++)
        //     {
        //         Assert.ThrowsAsync<ValidationException>(() =>
        //             _userService.CreateUser(new User {Login = (char) i + "GG", Email = "a@mail.ru"}));
        //         Assert.ThrowsAsync<ValidationException>(() =>
        //             _userService.CreateUser(new User {Login = "GG" + (char) i + "GG", Email = "a@mail.ru"}));
        //         Assert.ThrowsAsync<ValidationException>(() =>
        //             _userService.CreateUser(new User {Login = "GG" + (char) i, Email = "a@mail.ru"}));
        //         
        //         Assert.ThrowsAsync<ValidationException>(() =>
        //             _userService.UpdateUser(new User {Login = (char) i + "GG", Email = "a@mail.ru"}));
        //         Assert.ThrowsAsync<ValidationException>(() =>
        //             _userService.UpdateUser(new User {Login = "GG" + (char) i + "GG", Email = "a@mail.ru"}));
        //         Assert.ThrowsAsync<ValidationException>(() =>
        //             _userService.UpdateUser(new User {Login = "GG" + (char) i, Email = "a@mail.ru"}));
        //     }
        // }
        //
        // [Fact]
        // public void CreateAndUpdateUser_WithLoginWhichHasAlreadyInBase_ShouldThrowValidationException()
        // {
        //     _fakeUserRepository.Setup(repository => repository.ContainsLogin(It.IsAny<string>()).Result).Returns(true);
        //     Assert.ThrowsAsync<ValidationException>(() =>
        //         _userService.CreateUser(new User {Login = "Login", Email = "a@mail.ru"}));
        //     Assert.ThrowsAsync<ValidationException>(() =>
        //         _userService.UpdateUser(new User {Login = "Login", Email = "a@mail.ru"}));
        // }
        //
        //
        //
        // [Fact]
        // public void CreateAndUpdateUser_WithNullEmail_ShouldThrowValidationException()
        // {
        //     Assert.ThrowsAsync<ValidationException>(() =>
        //         _userService.CreateUser(new User {Login = "viktor", Email = null}));
        //     Assert.ThrowsAsync<ValidationException>(() =>
        //         _userService.UpdateUser(new User {Login = "viktor", Email = null}));
        // }
        //
        // [Fact]
        // public void CreateAndUpdateUser_WithEmptyEmail_ShouldThrowValidationException()
        // {
        //     Assert.ThrowsAsync<ValidationException>(() =>
        //         _userService.CreateUser(new User {Login = "viktor", Email = ""}));
        //     Assert.ThrowsAsync<ValidationException>(() =>
        //         _userService.UpdateUser(new User {Login = "viktor", Email = ""}));
        // }
        //
        // [Theory]
        // [InlineData("_userService.CreateUser")]
        // [InlineData("_userService.UpdateUser")]
        // public void CreateAndUpdateUser_WithEmailWithSpaces_ShouldThrowValidationException(Action a)
        // {
        //     a.Method(new User());
        //     
        //     Assert.ThrowsAsync<ValidationException>(() =>
        //         a.Method(new User {Login = "viktor", Email = "  "}));
        //     
        //     Assert.ThrowsAsync<ValidationException>(() =>
        //         _userService.CreateUser(new User {Login = "viktor", Email = "  "}));
        //     Assert.ThrowsAsync<ValidationException>(() =>
        //         _userService.CreateUser(new User {Login = "viktor", Email = " a@mail.ru"}));
        //     Assert.ThrowsAsync<ValidationException>(() =>
        //         _userService.CreateUser(new User {Login = "viktor", Email = "a@mail.ru "}));
        //     Assert.ThrowsAsync<ValidationException>(() =>
        //         _userService.CreateUser(new User {Login = "viktor", Email = " a@mail.ru "}));
        //     Assert.ThrowsAsync<ValidationException>(() =>
        //         _userService.CreateUser(new User {Login = "viktor", Email = "a t@mail.ru"}));
        //     
        //     Assert.ThrowsAsync<ValidationException>(() =>
        //         _userService.UpdateUser(new User {Login = "viktor", Email = "  "}));
        //     Assert.ThrowsAsync<ValidationException>(() =>
        //         _userService.UpdateUser(new User {Login = "viktor", Email = " a@mail.ru"}));
        //     Assert.ThrowsAsync<ValidationException>(() =>
        //         _userService.UpdateUser(new User {Login = "viktor", Email = "a@mail.ru "}));
        //     Assert.ThrowsAsync<ValidationException>(() =>
        //         _userService.UpdateUser(new User {Login = "viktor", Email = " a@mail.ru "}));
        //     Assert.ThrowsAsync<ValidationException>(() =>
        //         _userService.UpdateUser(new User {Login = "viktor", Email = "a t@mail.ru"}));
        // }
        //
        // [Theory]
        // [InlineData(0, 48)]
        // [InlineData(58, 64)]
        // [InlineData(91, 97)]
        // [InlineData(123, 256)]
        // public void CreateAndUpdateUser_WithEmailWithIncorrectFormat_ShouldThrowValidationException(int begin, int end)
        // {
        //     for (int i = begin; i < end; i++)
        //     {
        //         Assert.ThrowsAsync<ValidationException>(() =>
        //             _userService.CreateUser(new User {Login = "GG", Email = (char) i + "a@mail.ru"}));
        //         Assert.ThrowsAsync<ValidationException>(() =>
        //             _userService.CreateUser(new User {Login = "GGG", Email = "a" + (char) i + "gg@mail.ru"}));
        //         Assert.ThrowsAsync<ValidationException>(() =>
        //             _userService.CreateUser(new User {Login = "GG", Email = "a" + (char) i + "@mail.ru"}));
        //         
        //         Assert.ThrowsAsync<ValidationException>(() =>
        //             _userService.UpdateUser(new User {Login = "GG", Email = (char) i + "a@mail.ru"}));
        //         Assert.ThrowsAsync<ValidationException>(() =>
        //             _userService.UpdateUser(new User {Login = "GGG", Email = "a" + (char) i + "gg@mail.ru"}));
        //         Assert.ThrowsAsync<ValidationException>(() =>
        //             _userService.UpdateUser(new User {Login = "GG", Email = "a" + (char) i + "@mail.ru"}));
        //     }
        // }
        //
        [Fact]
        public void CreateAndUpdateUser_WithEmailNotMailRu_ShouldThrowValidationException()
        {
            Assert.ThrowsAsync<ValidationException>(() => _userService.CreateUser(new User {Login = "GG", Email = "amail.ru"}));
            Assert.ThrowsAsync<ValidationException>(() => _userService.CreateUser(new User {Login = "GG", Email = "a@mal.ru"}));
            Assert.ThrowsAsync<ValidationException>(() => _userService.CreateUser(new User {Login = "GG", Email = "a@mil.ru"}));
            Assert.ThrowsAsync<ValidationException>(() => _userService.CreateUser(new User {Login = "GG", Email = "a@mai.ru"}));
            Assert.ThrowsAsync<ValidationException>(() => _userService.CreateUser(new User {Login = "GG", Email = "a@mailru"}));
            Assert.ThrowsAsync<ValidationException>(() => _userService.CreateUser(new User {Login = "GG", Email = "a@mail"}));
            Assert.ThrowsAsync<ValidationException>(() => _userService.CreateUser(new User {Login = "GG", Email = "a@mail.ru"}));
            Assert.ThrowsAsync<ValidationException>(() => _userService.CreateUser(new User {Login = "GG", Email = "a@mail.u"}));
            Assert.ThrowsAsync<ValidationException>(() => _userService.CreateUser(new User {Login = "GG", Email = "a.ru"}));
            
            Assert.ThrowsAsync<ValidationException>(() => _userService.UpdateUser(new User {Login = "GG", Email = "amail.ru"}));
            Assert.ThrowsAsync<ValidationException>(() => _userService.UpdateUser(new User {Login = "GG", Email = "a@mal.ru"}));
            Assert.ThrowsAsync<ValidationException>(() => _userService.UpdateUser(new User {Login = "GG", Email = "a@mil.ru"}));
            Assert.ThrowsAsync<ValidationException>(() => _userService.UpdateUser(new User {Login = "GG", Email = "a@mai.ru"}));
            Assert.ThrowsAsync<ValidationException>(() => _userService.UpdateUser(new User {Login = "GG", Email = "a@mailru"}));
            Assert.ThrowsAsync<ValidationException>(() => _userService.UpdateUser(new User {Login = "GG", Email = "a@mail"}));
            Assert.ThrowsAsync<ValidationException>(() => _userService.UpdateUser(new User {Login = "GG", Email = "a@mail.ru"}));
            Assert.ThrowsAsync<ValidationException>(() => _userService.UpdateUser(new User {Login = "GG", Email = "a@mail.u"}));
            Assert.ThrowsAsync<ValidationException>(() => _userService.UpdateUser(new User {Login = "GG", Email = "a.ru"}));
        }
        //
        // [Fact]
        // public void CreateAndUpdateUser_WithEmailWhichHasAlreadyInBase_ShouldThrowValidationException()
        // {
        //     _fakeUserRepository.Setup(repository => repository.ContainsEmail(It.IsAny<string>()).Result).Returns(true);
        //     Assert.ThrowsAsync<ValidationException>(() =>
        //         _userService.CreateUser(new User {Login = "Login", Email = "a@mail.ru"}));
        //     
        //     _fakeUserRepository.Setup(repository => repository.ContainsEmail(It.IsAny<string>()).Result).Returns(true);
        //     Assert.ThrowsAsync<ValidationException>(() =>
        //         _userService.UpdateUser(new User {Login = "Login", Email = "a@mail.ru"}));
        // }
        //
        // [Fact]
        // public void CreateAndUpdateUser_SuccessPath_ShouldAddedInBase()
        // {
        //     Assert.ThrowsAsync<ValidationException>(() => _userService.CreateUser(new User {Login = "gg", Email = "a@mail.ru"}));
        // }
        //
        //
        
        
        
        [Fact]
        public async Task DeleteUser_UserWithBankAccount_ShouldThrowValidationException()
        {
            _fakeBankAccountRepository.Setup(repository => repository.HasBankAccounts(It.IsAny<int>()).Result)
                .Returns(true);
            await Assert.ThrowsAsync<ValidationException>(() =>
                _userService.DeleteUser(It.IsAny<int>()));
        }
        
        [Fact]
        public async Task DeleteUser_UserWithNotInBase_ShouldThrowValidationException()
        {
            _fakeUserRepository.Setup(repository => repository.DeleteUser(It.IsAny<int>())).Throws<Exception>();
            await Assert.ThrowsAsync<Exception>(() => _userService.DeleteUser(It.IsAny<int>()));
        }
        
        [Fact]
        public async Task DeleteUser_SuccessPath_ShouldDeleteUser()
        {
            _fakeBankAccountRepository.Setup(repository => repository.HasBankAccounts(It.IsAny<int>()).Result)
                .Returns(false);
            await _userService.DeleteUser(It.IsAny<int>());
            _fakeUserRepository.Setup(repository => repository.DeleteUser(It.IsAny<int>()));
            await _userService.DeleteUser(It.IsAny<int>());
        }

        [Fact]
        public async Task GetUser_UserWithNotInBase_ShouldThrowValidationException()
        {
            _fakeUserRepository.Setup(repository => repository.GetUser(It.IsAny<int>()).Result).Throws<Exception>();
            await Assert.ThrowsAsync<Exception>(() => _userService.GetUser(It.IsAny<int>()));
        }
        
        [Fact]
        public async Task GetUser_SuccessPath_ShouldGetUserFromBase()
        {
            _fakeUserRepository.Setup(repository => repository.GetUser(It.IsAny<int>()).Result).Returns(new 
                User{Id = It.IsAny<int>(), Email = It.IsAny<string>(), Login = It.IsAny<string>()});
            var result = await _userService.GetUser(4);
            Assert.True(result.Id == It.IsAny<int>() && result.Email == It.IsAny<string>() && result.Login == It.IsAny<string>());
        }

        [Fact]
        public async Task GetAllUsers_SuccessPath_ShouldGetAllUsersFromBase()
        {
            _fakeUserRepository.Setup(repository => repository.GetAllUsers().Result).Returns(new
                List<User>
                {
                    new User {Id = 1, Email = "a@mail.ru", Login = "viktor1"},
                    new User {Id = 2, Email = "b@mail.ru", Login = "viktor2"},
                    new User {Id = 3, Email = "c@mail.ru", Login = "viktor3"},
                    new User {Id = 4, Email = "d@mail.ru", Login = "viktor4"}
                });
            var result = await _userService.GetAllUsers();
            var element1 = result.First(it => it.Id == 1);
            var element2 = result.First(it => it.Id == 2);
            var element3 = result.First(it => it.Id == 3);
            var element4 = result.First(it => it.Id == 4);
            Assert.True(element1.Email == "a@mail.ru" && element1.Login == "viktor1" && element2.Email == "b@mail.ru" && element2.Login == "viktor2"
            && element3.Email == "c@mail.ru" && element3.Login == "viktor3" && element4.Email == "d@mail.ru" && element4.Login == "viktor4");
        }
    }
}