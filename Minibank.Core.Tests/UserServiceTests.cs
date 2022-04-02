using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using Minibank.Core.Domains.BankAccounts;
using Minibank.Core.Domains.BankAccounts.Repositories;
using Minibank.Core.Domains.Users;
using Minibank.Core.Domains.Users.Repositories;
using Minibank.Core.Domains.Users.Services;
using Moq;
using Xunit;

namespace Minibank.Core.Tests
{
    public class UserServiceTests
    {
        private readonly IUserService _userService;
        private readonly Mock<IUserRepository> _fakeUserRepository;
        private readonly Mock<IBankAccountRepository> _fakeBankAccountRepository;
        private readonly Mock<IUnitOfWork> _fakeUnitOfWork;
        private readonly Mock<AbstractValidator<User>> _fakeUserValidator;

        public UserServiceTests()
        {
            _fakeUserRepository = new Mock<IUserRepository>();
            _fakeBankAccountRepository = new Mock<IBankAccountRepository>();
            _fakeUnitOfWork = new Mock<IUnitOfWork>();
            _fakeUserValidator = new Mock<AbstractValidator<User>>();
            _userService = new UserService(_fakeUserRepository.Object, _fakeBankAccountRepository.Object,
                _fakeUnitOfWork.Object, _fakeUserValidator.Object);
        }
        
        [Fact]
        public void CreateUser_WithNullLogin_ShouldThrowValidationException()
        {
            Assert.ThrowsAsync<ValidationException>(() =>
                _userService.CreateUser(new User {Login = null, Email = "a@mail.ru"}));
        }

        [Fact]
        public void CreateUser_WithEmptyLogin_ShouldThrowValidationException()
        {
            Assert.ThrowsAsync<ValidationException>(() =>
                _userService.CreateUser(new User {Login = "", Email = "a@mail.ru"}));
        }

        [Fact]
        public void CreateUser_WithLoginWithSpaces_ShouldThrowValidationException()
        {
            Assert.ThrowsAsync<ValidationException>(() =>
                _userService.CreateUser(new User {Login = "  ", Email = "a@mail.ru"}));
            Assert.ThrowsAsync<ValidationException>(() =>
                _userService.CreateUser(new User {Login = " viktor", Email = "a@mail.ru"}));
            Assert.ThrowsAsync<ValidationException>(() =>
                _userService.CreateUser(new User {Login = "viktor ", Email = "a@mail.ru"}));
            Assert.ThrowsAsync<ValidationException>(() =>
                _userService.CreateUser(new User {Login = " viktor ", Email = "a@mail.ru"}));
            Assert.ThrowsAsync<ValidationException>(() =>
                _userService.CreateUser(new User {Login = "s s", Email = "a@mail.ru"}));
        }

        [Fact]
        public void CreateUser_WithLoginWithLengthMoreThan20_ShouldThrowValidationException()
        {
            Assert.ThrowsAsync<ValidationException>(() =>
                _userService.CreateUser(new User
                    {Login = "dddddddddddddddddddddddddddddddddddddddddddddddddddd", Email = "a@mail.ru"}));
        }

        [Theory]
        [InlineData(0, 48)]
        [InlineData(58, 65)]
        [InlineData(91, 97)]
        [InlineData(123, 256)]
        public void CreateUser_WithLoginWithIncorrectFormat_ShouldThrowValidationException(int begin, int end)
        {
            for (int i = begin; i < end; i++)
            {
                Assert.ThrowsAsync<ValidationException>(() =>
                    _userService.CreateUser(new User {Login = (char) i + "GG", Email = "a@mail.ru"}));
                Assert.ThrowsAsync<ValidationException>(() =>
                    _userService.CreateUser(new User {Login = "GG" + (char) i + "GG", Email = "a@mail.ru"}));
                Assert.ThrowsAsync<ValidationException>(() =>
                    _userService.CreateUser(new User {Login = "GG" + (char) i, Email = "a@mail.ru"}));
            }
        }

        [Fact]
        public void CreateUser_WithLoginWhichHasAlreadyInBase_ShouldThrowValidationException()
        {
            _fakeUserRepository.Setup(repository => repository.ContainsLogin(It.IsAny<string>()).Result).Returns(true);
            Assert.ThrowsAsync<ValidationException>(() =>
                _userService.CreateUser(new User {Login = "Login", Email = "a@mail.ru"}));
        }


        
        [Fact]
        public void CreateUser_WithNullEmail_ShouldThrowValidationException()
        {
            Assert.ThrowsAsync<ValidationException>(() =>
                _userService.CreateUser(new User {Login = "viktor", Email = null}));
        }

        [Fact]
        public void CreateUser_WithEmptyEmail_ShouldThrowValidationException()
        {
            Assert.ThrowsAsync<ValidationException>(() =>
                _userService.CreateUser(new User {Login = "viktor", Email = ""}));
        }
        
        [Fact]
        public void CreateUser_WithEmailWithSpaces_ShouldThrowValidationException()
        {
            Assert.ThrowsAsync<ValidationException>(() =>
                _userService.CreateUser(new User {Login = "viktor", Email = "  "}));
            Assert.ThrowsAsync<ValidationException>(() =>
                _userService.CreateUser(new User {Login = "viktor", Email = " a@mail.ru"}));
            Assert.ThrowsAsync<ValidationException>(() =>
                _userService.CreateUser(new User {Login = "viktor", Email = "a@mail.ru "}));
            Assert.ThrowsAsync<ValidationException>(() =>
                _userService.CreateUser(new User {Login = "viktor", Email = " a@mail.ru "}));
            Assert.ThrowsAsync<ValidationException>(() =>
                _userService.CreateUser(new User {Login = "viktor", Email = "a t@mail.ru"}));
        }
        
        [Theory]
        [InlineData(0, 48)]
        [InlineData(58, 64)]
        [InlineData(91, 97)]
        [InlineData(123, 256)]
        public void CreateUser_WithEmailWithIncorrectFormat_ShouldThrowValidationException(int begin, int end)
        {
            for (int i = begin; i < end; i++)
            {
                Assert.ThrowsAsync<ValidationException>(() =>
                    _userService.CreateUser(new User {Login = "GG", Email = (char) i + "a@mail.ru"}));
                Assert.ThrowsAsync<ValidationException>(() =>
                    _userService.CreateUser(new User {Login = "GGG", Email = "a" + (char) i + "gg@mail.ru"}));
                Assert.ThrowsAsync<ValidationException>(() =>
                    _userService.CreateUser(new User {Login = "GG", Email = "a" + (char) i + "@mail.ru"}));
            }
        }
        
        [Fact]
        public void CreateUser_WithEmailNotMailRu_ShouldThrowValidationException()
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
        }

        [Fact]
        public void CreateUser_WithEmailWhichHasAlreadyInBase_ShouldThrowValidationException()
        {
            _fakeUserRepository.Setup(repository => repository.ContainsEmail(It.IsAny<string>()).Result).Returns(true);
            Assert.ThrowsAsync<ValidationException>(() =>
                _userService.CreateUser(new User {Login = "Login", Email = "a@mail.ru"}));
        }
        
    }
}