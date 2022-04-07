using System;
using System.Threading.Tasks;
using FluentValidation;
using Minibank.Core.Domains.BankAccounts;
using Minibank.Core.Domains.BankAccounts.Services;
using Minibank.Core.Domains.BankAccounts.Validators;
using Minibank.Core.Domains.Users;
using Minibank.Core.Domains.Users.Repositories;
using Moq;
using Xunit;

namespace Minibank.Core.Tests.BankAccounts
{

    public class BankAccountValidatorTests
    {
        private readonly IValidator<BankAccount> _bankAccountValidator;
        private readonly Mock<IUserRepository> _fakeUserRepository; 
        public BankAccountValidatorTests()
        {
            _fakeUserRepository = new Mock<IUserRepository>();
            _bankAccountValidator = new BankAccountValidator();
        }
        
        [Fact]
        public async Task BankAccountValidator_BalanceLessThanZero_ShouldThrowValidationException()
        {
            await Assert.ThrowsAsync<FluentValidation.ValidationException>(() =>
                _bankAccountValidator.ValidateAndThrowAsync(new BankAccount{
                    UserId = 1, Balance = -1000, Currency = "RUB"
                }));
        }

        [Fact]
        public async Task BankAccountValidator_IncorrectCurrency_ShouldThrowValidationException()
        {
            await Assert.ThrowsAsync<FluentValidation.ValidationException>(() =>
                _bankAccountValidator.ValidateAndThrowAsync(new BankAccount{
                    UserId = 2, Balance = 1000, Currency = ""
                }));
            await Assert.ThrowsAsync<FluentValidation.ValidationException>(() =>
                _bankAccountValidator.ValidateAndThrowAsync(new BankAccount{
                    UserId = 2, Balance = 1000, Currency = "TRY"
                }));
            await Assert.ThrowsAsync<FluentValidation.ValidationException>(() =>
                _bankAccountValidator.ValidateAndThrowAsync(new BankAccount{
                    UserId = 2, Balance = 1000, Currency = "JPA"
                }));
        }

        [Fact]
        public async Task BankAccountValidator_SuccessPath_ShouldBeCompleteSuccessfully()
        {
            _fakeUserRepository.Setup(repository => repository.GetUser(2).Result)
                .Returns(new User {Id = 2, Email = "a@mail.ru", Login = "a"});
            await _bankAccountValidator.ValidateAndThrowAsync(new BankAccount{
                    UserId = 2, Balance = 1000, Currency = "RUB"
                });
            await _bankAccountValidator.ValidateAndThrowAsync(new BankAccount{
                    UserId = 2, Balance = 1000, Currency = "USD"
                });
            await _bankAccountValidator.ValidateAndThrowAsync(new BankAccount{
                    UserId = 2, Balance = 1000, Currency = "EUR"});
        }
    }
}