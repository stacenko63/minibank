using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using Minibank.Core.Domains.BankAccounts;
using Minibank.Core.Domains.BankAccounts.Repositories;
using Minibank.Core.Domains.BankAccounts.Services;
using Minibank.Core.Domains.BankAccounts.Validators;
using Minibank.Core.Domains.MoneyTransferHistory.Services;
using Minibank.Core.Domains.Users;
using Minibank.Core.Domains.Users.Repositories;
using Minibank.Core.Tests.Tests.Users;
using Moq;
using Xunit;
using BankAccountMessages = Minibank.Core.Domains.BankAccounts.Services.Messages;
using UserMessages = Minibank.Core.Domains.Users.Services.Messages;
using UserConstValues = Minibank.Core.Tests.Tests.Users.ConstValues;
using BankAccountConstValues = Minibank.Core.Tests.Tests.BankAccounts.ConstValues;

namespace Minibank.Core.Tests
{
    public class BankAccountServiceTests
    {
        private readonly Mock<IBankAccountRepository> _fakeBankAccountRepository;
        private readonly IBankAccountService _bankAccountService;
        private readonly Mock<ICurrencyConverter> _fakeCurrencyConverter;
        private readonly Mock<IUserRepository> _fakeUserRepository;
        private readonly BankAccountValidate _bankAccountValidator;

        public BankAccountServiceTests()
        {
            _bankAccountValidator = new BankAccountValidate();
            _fakeUserRepository = new Mock<IUserRepository>();
            _fakeBankAccountRepository = new Mock<IBankAccountRepository>();
            _fakeCurrencyConverter = new Mock<ICurrencyConverter>();
            _bankAccountService = new BankAccountService(_fakeBankAccountRepository.Object, _fakeUserRepository.Object,
                _fakeCurrencyConverter.Object,
                new Mock<IMoneyTransferHistoryService>().Object, new Mock<IUnitOfWork>().Object, _bankAccountValidator);
        }

        [Fact]
        public async Task CreateBankAccount_SuccessPath_ShouldCallRepository()
        {
            _fakeUserRepository.Setup(repository => repository.GetUser(ConstValues.UserId1))
                .ReturnsAsync(ConstValues.CorrectUser);
            await _bankAccountService.CreateBankAccount(ConstValues.UserId1, BankAccountConstValues.CorrectCurrency,
                BankAccountConstValues.CorrectBalance);

            _fakeBankAccountRepository.Verify(repository => repository.CreateBankAccount(ConstValues.UserId1,
                BankAccountConstValues.CorrectCurrency,
                BankAccountConstValues.CorrectBalance), Times.Once);
        }

        [Fact]
        public async Task CreateBankAccount_CheckValidatorWork_ShouldCallValidator()
        {
            _fakeUserRepository.Setup(repository => repository.GetUser(UserConstValues.UserId1))
                .ReturnsAsync(UserConstValues.CorrectUser);
            await _bankAccountService.CreateBankAccount(UserConstValues.UserId1, BankAccountConstValues.CorrectCurrency,
                BankAccountConstValues.CorrectBalance);

            Assert.True(_bankAccountValidator.IsCalled);
        }

        [Fact]
        public async Task CreateBankAccount_NonExistentUser_ShouldThrowValidationException()
        {
            _fakeUserRepository.Setup(repository => repository.GetUser(UserConstValues.UserId1))
                .ThrowsAsync(new ValidationException(UserMessages.NonExistentUser));
            var exception = await Assert.ThrowsAsync<ValidationException>(() =>
                _bankAccountService.CreateBankAccount(UserConstValues.UserId1, BankAccountConstValues.CorrectCurrency,
                    BankAccountConstValues.CorrectBalance));

            Assert.Equal(UserMessages.NonExistentUser, exception.Message);
        }

        [Fact]
        public async Task UpdateBankAccount_SuccessPath_ShouldCallRepository()
        {
            _fakeBankAccountRepository.Setup(repository => repository.GetAccount(BankAccountConstValues.BankAccountId1))
                .ReturnsAsync(
                    BankAccountConstValues.CorrectBankAccount);
            await _bankAccountService.UpdateBankAccount(BankAccountConstValues.CorrectBankAccount);

            _fakeBankAccountRepository.Verify(
                repository => repository.UpdateBankAccount(BankAccountConstValues.CorrectBankAccount), Times.Once);
        }

        [Fact]
        public async Task UpdateBankAccount_CheckValidatorWork_ShouldCallValidator()
        {
            _fakeBankAccountRepository.Setup(repository => repository.GetAccount(BankAccountConstValues.BankAccountId1))
                .ReturnsAsync(BankAccountConstValues.CorrectBankAccount);
            await _bankAccountService.UpdateBankAccount(BankAccountConstValues.CorrectBankAccount);

            Assert.True(_bankAccountValidator.IsCalled);
        }

        [Fact]
        public async Task UpdateBankAccount_NonExistentAccount_ShouldThrowValidationException()
        {
            _fakeBankAccountRepository.Setup(repository => repository.GetAccount(BankAccountConstValues.BankAccountId1))
                .ThrowsAsync(new ValidationException(BankAccountMessages.NonExistentAccount));

            var exception = await Assert.ThrowsAsync<ValidationException>(() =>
                _bankAccountService.UpdateBankAccount(BankAccountConstValues.CorrectBankAccount));
            Assert.Equal(BankAccountMessages.NonExistentAccount, exception.Message);
        }

        [Fact]
        public async Task GetBankAccount_NonExistentAccount_ShouldThrowValidationException()
        {
            _fakeBankAccountRepository.Setup(repository => repository.GetAccount(BankAccountConstValues.BankAccountId1))
                .ThrowsAsync(new ValidationException(BankAccountMessages.NonExistentAccount));

            var exception = await Assert.ThrowsAsync<ValidationException>(() =>
                _bankAccountService.GetAccount(BankAccountConstValues.BankAccountId1));
            Assert.Equal(BankAccountMessages.NonExistentAccount, exception.Message);
        }

        [Fact]
        public async Task GetBankAccount_SuccessPath_ShouldGetBankAccountFromBase()
        {
            _fakeBankAccountRepository.Setup(repository => repository.GetAccount(BankAccountConstValues.BankAccountId1))
                .ReturnsAsync(BankAccountConstValues.CorrectBankAccount);
            var result = await _bankAccountService.GetAccount(BankAccountConstValues.BankAccountId1);

            Assert.True((int) result.Balance == BankAccountConstValues.CorrectBalance);
            Assert.True(result.Currency == BankAccountConstValues.CorrectCurrency);
            Assert.True(result.IsOpen);
            Assert.True(result.UserId == UserConstValues.UserId1);
            Assert.True(result.Id == BankAccountConstValues.BankAccountId1);
            Assert.True(result.CloseAccountDate == BankAccountConstValues.CloseDateTime);
            Assert.True(result.OpenAccountDate == BankAccountConstValues.OpenDateTime);
        }


        [Theory]
        [InlineData(-100, 4, 5)]
        [InlineData(0, 4, 5)]
        public async Task GetCommission_WithIncorrectValue_ShouldThrowValidationException(int value, int fromAccountId,
            int toAccountId)
        {
            var exception = await Assert.ThrowsAsync<ValidationException>(() =>
                _bankAccountService.GetCommission(value, fromAccountId, toAccountId));

            Assert.Equal(BankAccountMessages.ZeroOrNegativeValue, exception.Message);
        }

        [Theory]
        [InlineData(false, true)]
        [InlineData(true, false)]
        [InlineData(true, true)]
        public async Task GetCommission_ForNonExistentAccounts_ShouldThrowValidationException(bool isThrowException1,
            bool isThrowException2)
        {
            if (isThrowException1)
            {
                _fakeBankAccountRepository
                    .Setup(repository => repository.GetAccount(BankAccountConstValues.BankAccountId1))
                    .ThrowsAsync(new ValidationException(BankAccountMessages.NonExistentAccount));
            }
            else
            {
                _fakeBankAccountRepository
                    .Setup(repository => repository.GetAccount(BankAccountConstValues.BankAccountId1))
                    .ReturnsAsync(new BankAccount { });
            }


            if (isThrowException2)
            {
                _fakeBankAccountRepository
                    .Setup(repository => repository.GetAccount(BankAccountConstValues.BankAccountId2))
                    .ThrowsAsync(new ValidationException(BankAccountMessages.NonExistentAccount));
            }
            else
            {
                _fakeBankAccountRepository
                    .Setup(repository => repository.GetAccount(BankAccountConstValues.BankAccountId2))
                    .ReturnsAsync(new BankAccount { });
            }

            var exception = await Assert.ThrowsAsync<ValidationException>(() =>
                _bankAccountService.GetCommission(BankAccountConstValues.CorrectBalance,
                    BankAccountConstValues.BankAccountId1, BankAccountConstValues.BankAccountId2));
            Assert.Equal(BankAccountMessages.NonExistentAccount, exception.Message);
        }

        [Theory]
        [InlineData(false, true)]
        [InlineData(true, false)]
        [InlineData(false, false)]
        public async Task GetCommission_ForAccountsWhichAreClosed_ShouldThrowValidationException(bool isOpened1,
            bool isOpened2)
        {
            _fakeBankAccountRepository.Setup(repository => repository.GetAccount(BankAccountConstValues.BankAccountId1))
                .ReturnsAsync(new BankAccount
                {
                    IsOpen = isOpened1
                });
            _fakeBankAccountRepository.Setup(repository => repository.GetAccount(BankAccountConstValues.BankAccountId2))
                .ReturnsAsync(new BankAccount
                {
                    IsOpen = isOpened2
                });

            var exception = await Assert.ThrowsAsync<ValidationException>(() =>
                _bankAccountService.GetCommission(BankAccountConstValues.CorrectBalance,
                    BankAccountConstValues.BankAccountId1, BankAccountConstValues.BankAccountId2));
            Assert.Equal(BankAccountMessages.GetCommissionForClosedAccount, exception.Message);
        }

        [Theory]
        [InlineData(20, 20, 1000, 0)]
        [InlineData(20, 19, 1000, 20)]
        [InlineData(20, 19, 123456, 2469)]
        public async Task GetCommission_SuccessPath_ShouldCheckValues(int userId1, int userId2, int value,
            int correctResult)
        {
            _fakeBankAccountRepository.Setup(repository => repository.GetAccount(BankAccountConstValues.BankAccountId1))
                .ReturnsAsync(new BankAccount
                {
                    UserId = userId1,
                    IsOpen = true
                });
            _fakeBankAccountRepository.Setup(repository => repository.GetAccount(BankAccountConstValues.BankAccountId2))
                .ReturnsAsync(new BankAccount
                {
                    UserId = userId2,
                    IsOpen = true
                });
            var result = await _bankAccountService.GetCommission(value,
                BankAccountConstValues.BankAccountId1, BankAccountConstValues.BankAccountId2);

            Assert.True((int) result == correctResult);
        }

        [Fact]
        public async Task CloseAccount_NonExistentAccount_ShouldThrowValidationException()
        {
            _fakeBankAccountRepository.Setup(repository => repository.GetAccount(BankAccountConstValues.BankAccountId1))
                .ThrowsAsync(new ValidationException(BankAccountMessages.NonExistentAccount));

            var exception = await Assert.ThrowsAsync<ValidationException>(() =>
                _bankAccountService.CloseAccount(BankAccountConstValues.BankAccountId1));
            Assert.Equal(BankAccountMessages.NonExistentAccount, exception.Message);
        }

        [Fact]
        public async Task CloseAccount_WithNotZeroBalance_ShouldThrowValidationException()
        {
            _fakeBankAccountRepository.Setup(repository => repository.GetAccount(BankAccountConstValues.BankAccountId1))
                .ReturnsAsync(new BankAccount
                {
                    Balance = BankAccountConstValues.CorrectBalance
                });

            var exception = await Assert.ThrowsAsync<ValidationException>(() =>
                _bankAccountService.CloseAccount(BankAccountConstValues.BankAccountId1));
            Assert.Equal(BankAccountMessages.CloseAccountWithNotZeroBalance, exception.Message);
        }

        [Fact]
        public async Task CloseAccount_WhichIsAlreadyClosed_ShouldThrowValidationException()
        {
            _fakeBankAccountRepository.Setup(repository => repository.GetAccount(BankAccountConstValues.BankAccountId1))
                .ReturnsAsync(new BankAccount
                {
                    Balance = 0,
                    IsOpen = false
                });

            var exception = await Assert.ThrowsAsync<ValidationException>(() =>
                _bankAccountService.CloseAccount(BankAccountConstValues.BankAccountId1));
            Assert.Equal(BankAccountMessages.CloseAccountThatAreAlreadyClosed, exception.Message);
        }

        [Fact]
        public async Task MakeMoneyTransfer_ToTheSameAccount_ShouldThrowValidationException()
        {
            var exception = await Assert.ThrowsAsync<ValidationException>(() =>
                _bankAccountService.MakeMoneyTransfer(BankAccountConstValues.CorrectBalance,
                    BankAccountConstValues.BankAccountId1, BankAccountConstValues.BankAccountId1));

            Assert.Equal(BankAccountMessages.TransferMoneyToTheSameAccount, exception.Message);
        }

        [Theory]
        [InlineData(-100, 4, 5)]
        [InlineData(0, 4, 5)]
        public async Task MakeMoneyTransfer_ValueLessThanZeroOrEqualToZero_ShouldThrowValidationException(int value,
            int fromAccountId, int toAccountId)
        {
            var exception = await Assert.ThrowsAsync<ValidationException>(() =>
                _bankAccountService.MakeMoneyTransfer(value, fromAccountId, toAccountId));

            Assert.Equal(BankAccountMessages.ZeroOrNegativeValue, exception.Message);
        }

        [Theory]
        [InlineData(false, true)]
        [InlineData(true, false)]
        [InlineData(true, true)]
        public async Task MakeMoneyTransfer_ForNonExistentAccounts_ShouldThrowValidationException(
            bool isThrowException1, bool isThrowException2)
        {
            if (isThrowException1)
            {
                _fakeBankAccountRepository
                    .Setup(repository => repository.GetAccount(BankAccountConstValues.BankAccountId1))
                    .ThrowsAsync(new ValidationException(BankAccountMessages.NonExistentAccount));
            }
            else
            {
                _fakeBankAccountRepository
                    .Setup(repository => repository.GetAccount(BankAccountConstValues.BankAccountId1))
                    .ReturnsAsync(new BankAccount { });
            }


            if (isThrowException2)
            {
                _fakeBankAccountRepository
                    .Setup(repository => repository.GetAccount(BankAccountConstValues.BankAccountId2))
                    .ThrowsAsync(new ValidationException(BankAccountMessages.NonExistentAccount));
            }
            else
            {
                _fakeBankAccountRepository
                    .Setup(repository => repository.GetAccount(BankAccountConstValues.BankAccountId2))
                    .ReturnsAsync(new BankAccount { });
            }

            var exception = await Assert.ThrowsAsync<ValidationException>(() =>
                _bankAccountService.MakeMoneyTransfer(BankAccountConstValues.CorrectBalance,
                    BankAccountConstValues.BankAccountId1, BankAccountConstValues.BankAccountId2));
            Assert.Equal(BankAccountMessages.NonExistentAccount, exception.Message);



        }

        [Fact]
        public async Task MakeMoneyTransfer_WithMissingAmount_ShouldThrowValidationException()
        {
            _fakeBankAccountRepository.Setup(repository => repository.GetAccount(BankAccountConstValues.BankAccountId1))
                .ReturnsAsync(new BankAccount
                {
                    Balance = 1000
                });

            var exception = await Assert.ThrowsAsync<ValidationException>(() =>
                _bankAccountService.MakeMoneyTransfer(100000, BankAccountConstValues.BankAccountId1, 5));
            Assert.Equal(BankAccountMessages.NotEnoughBalance, exception.Message);
        }

        [Theory]
        [InlineData(false, true)]
        [InlineData(true, false)]
        [InlineData(false, false)]
        public async Task MakeMoneyTransfer_ForAccountsWhichAreClosed_ShouldThrowValidationException(bool isOpened1,
            bool isOpened2)
        {
            _fakeBankAccountRepository.Setup(repository => repository.GetAccount(BankAccountConstValues.BankAccountId1))
                .ReturnsAsync(new BankAccount
                {
                    IsOpen = isOpened1,
                    Balance = BankAccountConstValues.CorrectBalance
                });
            _fakeBankAccountRepository.Setup(repository => repository.GetAccount(BankAccountConstValues.BankAccountId2))
                .ReturnsAsync(new BankAccount
                {
                    IsOpen = isOpened2,
                    Balance = BankAccountConstValues.CorrectBalance
                });


            var exception = await Assert.ThrowsAsync<ValidationException>(() =>
                _bankAccountService.MakeMoneyTransfer(BankAccountConstValues.CorrectBalance,
                    BankAccountConstValues.BankAccountId1, BankAccountConstValues.BankAccountId2));
            Assert.Equal(BankAccountMessages.MoneyTransferBetweenClosedAccounts, exception.Message);



        }

        [Theory]
        [InlineData(false, 80000, 120000)]
        [InlineData(true, 60000, 119600)]
        public async Task MakeMoneyTransfer_SuccessPath_ShouldTransferMoney(bool differentUserId,
            int resultBalance1, int resultBalance2)
        {
            const int moneyTransfer = 20000;


            _fakeBankAccountRepository.Setup(repository => repository.GetAccount(BankAccountConstValues.BankAccountId1))
                .ReturnsAsync(BankAccountConstValues.CorrectBankAccount);


            if (differentUserId)
            {
                _fakeBankAccountRepository
                    .Setup(repository => repository.GetAccount(BankAccountConstValues.BankAccountId2)).ReturnsAsync(
                        new BankAccount
                        {
                            IsOpen = true,
                            Balance = BankAccountConstValues.CorrectBalance,
                            Currency = BankAccountConstValues.CorrectCurrency,
                            UserId = UserConstValues.UserId2,
                            Id = BankAccountConstValues.BankAccountId2
                        });
            }
            else
            {
                _fakeBankAccountRepository
                    .Setup(repository => repository.GetAccount(BankAccountConstValues.BankAccountId2)).ReturnsAsync(
                        new BankAccount
                        {
                            IsOpen = true,
                            Balance = BankAccountConstValues.CorrectBalance,
                            Currency = BankAccountConstValues.CorrectCurrency,
                            UserId = UserConstValues.UserId1,
                            Id = BankAccountConstValues.BankAccountId2
                        });
            }



            _fakeCurrencyConverter.Setup(converter => converter.GetValueInOtherCurrency(moneyTransfer,
                BankAccountConstValues.CorrectCurrency, BankAccountConstValues.CorrectCurrency)).ReturnsAsync(20000);
            _fakeCurrencyConverter.Setup(converter => converter.GetValueInOtherCurrency(0,
                BankAccountConstValues.CorrectCurrency, BankAccountConstValues.CorrectCurrency)).ReturnsAsync(0);
            _fakeCurrencyConverter.Setup(converter => converter.GetValueInOtherCurrency(400,
                BankAccountConstValues.CorrectCurrency, BankAccountConstValues.CorrectCurrency)).ReturnsAsync(400);


            await _bankAccountService.MakeMoneyTransfer(moneyTransfer,
                BankAccountConstValues.BankAccountId1, BankAccountConstValues.BankAccountId2);

            var account1 = await _bankAccountService.GetAccount(BankAccountConstValues.BankAccountId1);
            var account2 = await _bankAccountService.GetAccount(BankAccountConstValues.BankAccountId2);

            Assert.True((int) account1.Balance == resultBalance1);
            Assert.True((int) account2.Balance == resultBalance2);
        }

        public class BankAccountValidate : IValidator<BankAccount>
        {
            public bool IsCalled { get; set; }

            public BankAccountValidate()
            {
                IsCalled = false;
            }

            public ValidationResult Validate(IValidationContext context)
            {
                IsCalled = true;
                return new ValidationResult();
            }

            public async Task<ValidationResult> ValidateAsync(IValidationContext context,
                CancellationToken cancellation = new CancellationToken())
            {
                IsCalled = true;
                return new ValidationResult();
            }

            public IValidatorDescriptor CreateDescriptor()
            {
                return new ValidatorDescriptor<BankAccount>(new List<IValidationRule>());
            }

            public bool CanValidateInstancesOfType(Type type)
            {
                return true;
            }

            public ValidationResult Validate(BankAccount instance)
            {
                IsCalled = true;
                return new ValidationResult();
            }

            public async Task<ValidationResult> ValidateAsync(BankAccount instance,
                CancellationToken cancellation = new CancellationToken())
            {
                IsCalled = true;
                return new ValidationResult();
            }
        }
    }
}