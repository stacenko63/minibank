using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Minibank.Core.Domains.BankAccounts.Repositories;
using Minibank.Core.Domains.Users;
using Minibank.Core.Domains.Users.Repositories;
using Minibank.Core.Domains.Users.Services;
using Minibank.Core.Domains.Users.Validators;
using Moq;
using Xunit;
using Messages = Minibank.Core.Domains.Users.Validators.Messages;

namespace Minibank.Core.Tests.Users
{
    public class UserValidatorTests
    {
        
        private readonly IValidator<User> _userValidator;
        private readonly Mock<IUserRepository> _fakeUserRepository;

        public UserValidatorTests()
        {
            _fakeUserRepository = new Mock<IUserRepository>();
            _userValidator = new UserValidator();
        }

        [Fact]
        public async Task UserValidator_EmptyLogin_ShouldThrowValidationException()
        {
            var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(() =>
                _userValidator.ValidateAndThrowAsync(new User {Login = "", Email = "a@mail.ru"}));  
            Assert.Contains(Messages.EmptyLogin, exception.Message);
        }
        
        [Theory]
        [InlineData("  ")]
        [InlineData(" viktor")]
        [InlineData("viktor ")]
        [InlineData(" viktor ")]
        [InlineData("s s")]
        
        public async Task UserValidator_LoginWithSpaces_ShouldThrowValidationException(string login)
        {
            var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(() =>
                _userValidator.ValidateAndThrowAsync(new User {Login = login, Email = "a@mail.ru"}));
            Assert.Contains(Messages.LoginWithSpaces, exception.Message);
        }
        
        [Fact]
        public async Task UserValidator_LoginWithLengthMoreThan20_ShouldThrowValidationException()
        {
            var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(() =>
                _userValidator.ValidateAndThrowAsync(new User {Login = "dddddddddddddddddddddddddddddddddddddddddddddddddddd", 
                    Email = "a@mail.ru"}));
            Assert.Contains(Messages.LoginWithLengthMoreThan20, exception.Message);
        }
        
        [Theory]
        [InlineData(33, 48)]
        [InlineData(58, 65)]
        [InlineData(91, 97)]
        [InlineData(123, 256)]
        public async Task UserValidator_LoginWithIncorrectFormat_ShouldThrowValidationException(int begin, int end)
        {
            const string email = "a@mail.ru";
            for (int i = begin; i < end; i++)
            {
                var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(() =>
                    _userValidator.ValidateAndThrowAsync(new User {Login = (char) i + "GG", Email = email}));
                Assert.Contains(Messages.LoginFormat, exception.Message);
                exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(() =>
                    _userValidator.ValidateAndThrowAsync(new User {Login = "GG" + (char) i + "GG", Email = email}));
                Assert.Contains(Messages.LoginFormat, exception.Message);
                exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(() =>
                    _userValidator.ValidateAndThrowAsync(new User {Login = "GG" + (char) i, Email = email}));
                Assert.Contains(Messages.LoginFormat, exception.Message);
            }
        }

        [Theory]
        [InlineData(48,58)]
        [InlineData(65,91)]
        [InlineData(97,123)]
        public async Task UserValidator_SuccessPathsLogin_ShouldBeCompleteSuccessfully(int begin, int end)
        {
            const string email = "a@mail.ru";
            for (int i = begin; i < end; i++)
            {
                await _userValidator.ValidateAndThrowAsync(new User {Login = (char) i + "GG", Email = email});
                await _userValidator.ValidateAndThrowAsync(new User {Login = "GG" + (char) i + "GG", Email = email});
                await _userValidator.ValidateAndThrowAsync(new User {Login = "GG" + (char) i, Email = email});
            }
        }
        

        [Fact]
        public async Task UserValidator_EmptyEmail_ShouldThrowValidationException()
        {
            var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(() =>
                _userValidator.ValidateAndThrowAsync(new User {Login = "Viktor", Email = ""}));
            Assert.Contains(Messages.EmptyEmail, exception.Message);
        }
        
        [Theory]
        [InlineData("  ")]
        [InlineData(" a@mail.ru")]
        [InlineData("a@mail.ru ")]
        [InlineData(" a@mail.ru ")]
        [InlineData("a t@mail.ru")]
        public async Task UserValidator_EmailWithSpaces_ShouldThrowValidationException(string email)
        {
            var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(() =>
                _userValidator.ValidateAndThrowAsync(new User {Login = "Viktor", Email = email}));
            Assert.Contains(Messages.EmailWithSpaces, exception.Message);
        }
        
        [Theory]
        [InlineData(33, 46)]
        [InlineData(47, 48)]
        [InlineData(58, 64)]
        [InlineData(91, 97)]
        [InlineData(123, 256)]
        public async Task UserValidator_EmailWithIncorrectFormat_ShouldThrowValidationException(int begin, int end)
        {
            const string login = "viktor";
            for (int i = begin; i < end; i++)
            {
                var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(() =>
                    _userValidator.ValidateAndThrowAsync(new User {Login = login, Email = (char) i + "a@mail.ru"}));
                Assert.Contains(Messages.EmailFormat, exception.Message);
                exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(() =>
                    _userValidator.ValidateAndThrowAsync(new User {Login = login, Email = "a" + (char) i + "gg@mail.ru"}));
                Assert.Contains(Messages.EmailFormat, exception.Message);
                exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(() =>
                    _userValidator.ValidateAndThrowAsync(new User {Login = login, Email = "a" + (char) i + "@mail.ru"}));
                Assert.Contains(Messages.EmailFormat, exception.Message);
            }
        }

        [Theory]
        [InlineData("amail.ru")]
        [InlineData("a@mal.ru")]
        [InlineData("a@mil.ru")]
        [InlineData("a@mai.ru")]
        [InlineData("a@mailru")]
        [InlineData("a@mail")]
        [InlineData("a@mail.u")]
        [InlineData("a.ru")]
        public async Task UserValidator_EmailNotMailRu_ShouldThrowValidationException(string email)
        {
            var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(() => 
                _userValidator.ValidateAndThrowAsync(new User {Login = "GG", Email = email}));
            Assert.Contains(Messages.EmailNotMailRu, exception.Message);
        }

        [Fact]
        public async Task UserValidator_SuccessPath_ShouldBeCompleteSuccessfully()
        {
            const string login = "viktor", email = "a@mail.ru";
            _fakeUserRepository.Setup(repository => repository.ContainsLogin(login).Result).Returns(false);
            _fakeUserRepository.Setup(repository => repository.ContainsEmail(email).Result).Returns(false);
            await _userValidator.ValidateAndThrowAsync(new User {Login = login, Email = email});
        }
        
        [Theory]
        [InlineData(48,58)]
        [InlineData(64,91)]
        [InlineData(97,123)]
        public async Task UserValidator_SuccessPathsEmail_ShouldBeCompleteSuccessfully(int begin, int end)
        {
            const string login = "viktor";
            for (int i = begin; i < end; i++)
            {
                await _userValidator.ValidateAndThrowAsync(new User {Login = login, Email = (char) i + "a@mail.ru"});
                await _userValidator.ValidateAndThrowAsync(new User {Login = login, Email = "a" + (char) i + "gg@mail.ru"});
                await _userValidator.ValidateAndThrowAsync(new User {Login = login, Email = "a" + (char) i + "@mail.ru"});
            }
        }
        
    }
}