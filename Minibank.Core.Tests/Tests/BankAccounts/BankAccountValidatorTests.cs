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
using Messages = Minibank.Core.Domains.BankAccounts.Validators.Messages;

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
            var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(() =>
                _bankAccountValidator.ValidateAndThrowAsync(new BankAccount{
                    UserId = 1, Balance = -1000, Currency = "RUB"
                }));
            Assert.Contains(Messages.NegativeStartBalance, exception.Message);
        }

        [Theory]
        [InlineData("")]
        [InlineData("TRY")]
        [InlineData("JPA")]
        public async Task BankAccountValidator_IncorrectCurrency_ShouldThrowValidationException(string currency)
        {
            var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(() =>
                _bankAccountValidator.ValidateAndThrowAsync(new BankAccount{
                    UserId = 2, Balance = 1000, Currency = currency
                }));
            Assert.Contains(Messages.NotPermittedCurrency, exception.Message);
        }

        [Theory]
        [InlineData("RUB")]
        [InlineData("USD")]
        [InlineData("EUR")]
        public async Task BankAccountValidator_SuccessPath_ShouldBeCompleteSuccessfully(string currency)
        {
            const int userId = 2;
            _fakeUserRepository.Setup(repository => repository.GetUser(userId).Result)
                .Returns(new User {Id = userId, Email = "a@mail.ru", Login = "a"});
            await _bankAccountValidator.ValidateAndThrowAsync(new BankAccount{
                    UserId = userId, Balance = 1000, Currency = currency
                });
        }
    }
}