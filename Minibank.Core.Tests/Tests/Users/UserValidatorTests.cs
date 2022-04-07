using System;
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

namespace Minibank.Core.Tests.Users
{
    public class UserValidatorTests
    {
        
        private readonly IValidator<User> _userValidator;
        private readonly Mock<IUserRepository> _fakeUserRepository;

        public UserValidatorTests()
        {
            _fakeUserRepository = new Mock<IUserRepository>();
            _userValidator = new UserValidator(_fakeUserRepository.Object);
        }

        [Fact]
        public async Task UserValidator_EmptyLogin_ShouldThrowValidationException()
        {
            await Assert.ThrowsAsync<FluentValidation.ValidationException>(() =>
                _userValidator.ValidateAndThrowAsync(new User {Login = "", Email = "a@mail.ru"}));
        }
        
        [Fact]
        public async Task UserValidator_LoginWithSpaces_ShouldThrowValidationException()
        {
            await Assert.ThrowsAsync<FluentValidation.ValidationException>(() =>
                _userValidator.ValidateAndThrowAsync(new User {Login = "  ", Email = "a@mail.ru"}));
            await Assert.ThrowsAsync<FluentValidation.ValidationException>(() =>
                _userValidator.ValidateAndThrowAsync(new User {Login = " viktor", Email = "a@mail.ru"}));
            await Assert.ThrowsAsync<FluentValidation.ValidationException>(() =>
                _userValidator.ValidateAndThrowAsync(new User {Login = "viktor ", Email = "a@mail.ru"}));
            await Assert.ThrowsAsync<FluentValidation.ValidationException>(() =>
                _userValidator.ValidateAndThrowAsync(new User {Login = " viktor ", Email = "a@mail.ru"}));
            await Assert.ThrowsAsync<FluentValidation.ValidationException>(() =>
                _userValidator.ValidateAndThrowAsync(new User {Login = "s s", Email = "a@mail.ru"}));
        }
        
        [Fact]
        public async Task UserValidator_LoginWithLengthMoreThan20_ShouldThrowValidationException()
        {
            await Assert.ThrowsAsync<FluentValidation.ValidationException>(() =>
                _userValidator.ValidateAndThrowAsync(new User {Login = "dddddddddddddddddddddddddddddddddddddddddddddddddddd", 
                    Email = "a@mail.ru"}));
        }
        
        [Theory]
        [InlineData(32, 48)]
        [InlineData(58, 65)]
        [InlineData(91, 97)]
        [InlineData(123, 256)]
        public async Task UserValidator_LoginWithIncorrectFormat_ShouldThrowValidationException(int begin, int end)
        {
            for (int i = begin; i < end; i++)
            {
                await Assert.ThrowsAsync<FluentValidation.ValidationException>(() =>
                    _userValidator.ValidateAndThrowAsync(new User {Login = (char) i + "GG", Email = "a@mail.ru"}));
                await Assert.ThrowsAsync<FluentValidation.ValidationException>(() =>
                    _userValidator.ValidateAndThrowAsync(new User {Login = "GG" + (char) i + "GG", Email = "a@mail.ru"}));
                await Assert.ThrowsAsync<FluentValidation.ValidationException>(() =>
                    _userValidator.ValidateAndThrowAsync(new User {Login = "GG" + (char) i, Email = "a@mail.ru"}));
            }
        }
        
        [Fact]
        public async Task UserValidator_LoginWhichHasAlreadyInBase_ShouldThrowValidationException()
        {
            _fakeUserRepository.Setup(repository => repository.ContainsLogin("Viktor").Result).Returns(true);
            await Assert.ThrowsAsync<FluentValidation.ValidationException>(() =>
                _userValidator.ValidateAndThrowAsync(new User {Login = "Viktor", Email = "a@mail.ru"}));
        }
        
        [Theory]
        [InlineData(48,58)]
        [InlineData(65,91)]
        [InlineData(97,123)]
        public async Task UserValidator_SuccessPathsLogin_ShouldBeCompleteSuccessfully(int begin, int end)
        {
            for (int i = begin; i < end; i++)
            {
                await _userValidator.ValidateAndThrowAsync(new User {Login = (char) i + "GG", Email = "a@mail.ru"});
                await _userValidator.ValidateAndThrowAsync(new User {Login = "GG" + (char) i + "GG", Email = "a@mail.ru"});
                await _userValidator.ValidateAndThrowAsync(new User {Login = "GG" + (char) i, Email = "a@mail.ru"});
            }
        }
        

        [Fact]
        public async Task UserValidator_EmptyEmail_ShouldThrowValidationException()
        {
            await Assert.ThrowsAsync<FluentValidation.ValidationException>(() =>
                _userValidator.ValidateAndThrowAsync(new User {Login = "Viktor", Email = ""}));
        }
        
        [Fact]
        public async Task UserValidator_EmailWithSpaces_ShouldThrowValidationException()
        {
            await Assert.ThrowsAsync<FluentValidation.ValidationException>(() =>
                _userValidator.ValidateAndThrowAsync(new User {Login = "Viktor", Email = "  "}));
            await Assert.ThrowsAsync<FluentValidation.ValidationException>(() =>
                _userValidator.ValidateAndThrowAsync(new User {Login = "Viktor", Email = " a@mail.ru"}));
            await Assert.ThrowsAsync<FluentValidation.ValidationException>(() =>
                _userValidator.ValidateAndThrowAsync(new User {Login = "Viktor", Email = "a@mail.ru "}));
            await Assert.ThrowsAsync<FluentValidation.ValidationException>(() =>
                _userValidator.ValidateAndThrowAsync(new User {Login = "Viktor", Email = " a@mail.ru "}));
            await Assert.ThrowsAsync<FluentValidation.ValidationException>(() =>
                _userValidator.ValidateAndThrowAsync(new User {Login = "Viktor", Email = "a t@mail.ru"}));
        }
        
        [Theory]
        [InlineData(32, 46)]
        [InlineData(47, 48)]
        [InlineData(58, 64)]
        [InlineData(91, 97)]
        [InlineData(123, 256)]
        public async Task UserValidator_EmailWithIncorrectFormat_ShouldThrowValidationException(int begin, int end)
        {
            for (int i = begin; i < end; i++)
            {
                await Assert.ThrowsAsync<FluentValidation.ValidationException>(() =>
                    _userValidator.ValidateAndThrowAsync(new User {Login = "Viktor", Email = (char) i + "a@mail.ru"}));
                await Assert.ThrowsAsync<FluentValidation.ValidationException>(() =>
                    _userValidator.ValidateAndThrowAsync(new User {Login = "Viktor", Email = "a" + (char) i + "gg@mail.ru"}));
                await Assert.ThrowsAsync<FluentValidation.ValidationException>(() =>
                    _userValidator.ValidateAndThrowAsync(new User {Login = "Viktor", Email = "a" + (char) i + "@mail.ru"}));
            }
        }

        [Fact]
        public async Task UserValidator_EmailWhichHasAlreadyInBase_ShouldThrowValidationException()
        {
            _fakeUserRepository.Setup(repository => repository.ContainsEmail("a@mail.ru").Result).Returns(true);
            await Assert.ThrowsAsync<FluentValidation.ValidationException>(() =>
                _userValidator.ValidateAndThrowAsync(new User {Login = "Viktor", Email = "a@mail.ru"}));
            
        }
        
        [Fact]
        public async Task UserValidator_EmailNotMailRu_ShouldThrowValidationException()
        {
            await Assert.ThrowsAsync<FluentValidation.ValidationException>(() => _userValidator.ValidateAndThrowAsync(new User {Login = "GG", Email = "amail.ru"}));
            await Assert.ThrowsAsync<FluentValidation.ValidationException>(() => _userValidator.ValidateAndThrowAsync(new User {Login = "GG", Email = "a@mal.ru"}));
            await Assert.ThrowsAsync<FluentValidation.ValidationException>(() => _userValidator.ValidateAndThrowAsync(new User {Login = "GG", Email = "a@mil.ru"}));
            await Assert.ThrowsAsync<FluentValidation.ValidationException>(() => _userValidator.ValidateAndThrowAsync(new User {Login = "GG", Email = "a@mai.ru"}));
            await Assert.ThrowsAsync<FluentValidation.ValidationException>(() => _userValidator.ValidateAndThrowAsync(new User {Login = "GG", Email = "a@mailru"}));
            await Assert.ThrowsAsync<FluentValidation.ValidationException>(() => _userValidator.ValidateAndThrowAsync(new User {Login = "GG", Email = "a@mail"}));
            await Assert.ThrowsAsync<FluentValidation.ValidationException>(() => _userValidator.ValidateAndThrowAsync(new User {Login = "GG", Email = "a@mail.u"}));
            await Assert.ThrowsAsync<FluentValidation.ValidationException>(() => _userValidator.ValidateAndThrowAsync(new User {Login = "GG", Email = "a.ru"}));
        }

        [Fact]
        public async Task UserValidator_SuccessPath_ShouldBeCompleteSuccessfully()
        {
            _fakeUserRepository.Setup(repository => repository.ContainsLogin("viktor").Result).Returns(false);
            _fakeUserRepository.Setup(repository => repository.ContainsEmail("a@mail.ru").Result).Returns(false);
            await _userValidator.ValidateAndThrowAsync(new User {Login = "viktor", Email = "a@mail.ru"});
        }
        
        [Theory]
        [InlineData(48,58)]
        [InlineData(64,91)]
        [InlineData(97,123)]
        public async Task UserValidator_SuccessPathsEmail_ShouldBeCompleteSuccessfully(int begin, int end)
        {
            for (int i = begin; i < end; i++)
            {
                await _userValidator.ValidateAndThrowAsync(new User {Login = "Viktor1", Email = (char) i + "a@mail.ru"});
                await _userValidator.ValidateAndThrowAsync(new User {Login = "Viktor2", Email = "a" + (char) i + "gg@mail.ru"});
                await _userValidator.ValidateAndThrowAsync(new User {Login = "Viktor3", Email = "a" + (char) i + "@mail.ru"});
            }
        }
        
    }
}