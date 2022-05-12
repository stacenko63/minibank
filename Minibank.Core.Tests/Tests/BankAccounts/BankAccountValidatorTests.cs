using System;
using System.Threading.Tasks;
using FluentValidation;
using Minibank.Core.Domains.BankAccounts;
using Minibank.Core.Domains.BankAccounts.Services;
using Minibank.Core.Domains.BankAccounts.Validators;
using Minibank.Core.Domains.Users;
using Minibank.Core.Domains.Users.Repositories;
using Minibank.Core.Tests.Tests.BankAccounts;
using Moq;
using Xunit;
using Messages = Minibank.Core.Domains.BankAccounts.Validators.Messages;
using UserConstValues = Minibank.Core.Tests.Tests.Users.ConstValues;
using BankAccountConstValues = Minibank.Core.Tests.Tests.BankAccounts.ConstValues;

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
                    UserId = UserConstValues.UserId1, 
                    Balance = BankAccountConstValues.NegativeBalance, 
                    Currency = BankAccountConstValues.CorrectCurrency
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
                    UserId = UserConstValues.UserId1, 
                    Balance = BankAccountConstValues.CorrectBalance, 
                    Currency = currency
                }));
            
            Assert.Contains(Messages.NotPermittedCurrency, exception.Message);
        }

        [Theory]
        [InlineData("RUB")]
        [InlineData("USD")]
        [InlineData("EUR")]
        public async Task BankAccountValidator_SuccessPath_ShouldBeCompleteSuccessfully(string currency)
        {
            _fakeUserRepository.Setup(repository => repository.GetUser(UserConstValues.UserId1))
                .ReturnsAsync(UserConstValues.CorrectUser);
            
            await _bankAccountValidator.ValidateAndThrowAsync(new BankAccount{
                    UserId = UserConstValues.UserId1, 
                    Balance = BankAccountConstValues.CorrectBalance, 
                    Currency = currency
                });
        }
        
    }
}