using System;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using Minibank.Core.Domains.BankAccounts;
using Minibank.Core.Domains.BankAccounts.Repositories;
using Minibank.Core.Domains.BankAccounts.Services;
using Minibank.Core.Domains.BankAccounts.Validators;
using Minibank.Core.Domains.MoneyTransferHistory.Services;
using Minibank.Core.Domains.Users;
using Minibank.Core.Domains.Users.Repositories;
using Moq;
using Xunit;
using BankAccountMessages = Minibank.Core.Domains.BankAccounts.Services.Messages;
using UserMessages = Minibank.Core.Domains.Users.Services.Messages;

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
        public async Task CreateBankAccount_SuccessPath_ShouldBeCompleteSuccessfully()
        {
            const int userId = 2, startBalance = 1000;
            const string currency = "RUB", email = "a@mail.ru", login = "Viktor";
            _fakeUserRepository.Setup(repository => repository.GetUser(userId).Result).Returns(new
                User {Login = login, Email = email, Id = userId});
            await _bankAccountService.CreateBankAccount(userId, currency, startBalance);
            _fakeBankAccountRepository.Verify(repository => repository.CreateBankAccount(userId, currency, startBalance), Times.Once);
        }
        
        [Fact]
        public async Task CreateBankAccount_CheckValidatorWork_ShouldCheckValidatorCall()
        {
            const int id = 3;
            _fakeUserRepository.Setup(repository => repository.GetUser(id).Result).Returns(new User {Id = id});
            await _bankAccountService.CreateBankAccount(id, "RUB", 0);
            Assert.True(_bankAccountValidator.IsCalled);
        }   
        
        [Fact]
        public async Task CreateBankAccount_NonExistentUser_ShouldThrowValidationException()
        {
            const int userId = 2, balance = 1000;
            const string currency = "RUB";
            _fakeUserRepository.Setup(repository => repository.GetUser(userId).Result).
                Throws(new ValidationException(UserMessages.NonExistentUser));
            var exception = await Assert.ThrowsAsync<ValidationException>(() =>
                _bankAccountService.CreateBankAccount(userId, currency, balance));
            Assert.Equal(UserMessages.NonExistentUser, exception.Message);
        }
        
        [Fact]
        public async Task UpdateBankAccount_SuccessPath_ShouldBeCompleteSuccessfully()
        {
            const int bankAccountId = 3;
            var bankAccount = new BankAccount
            {
                Balance = 1000, 
                Currency = "RUB", 
                IsOpen = true, 
                UserId = 2, 
                Id = bankAccountId,
                CloseAccountDate = DateTime.MinValue,
                OpenAccountDate = DateTime.Now
            };
            _fakeBankAccountRepository.Setup(repository => repository.GetAccount(bankAccountId).Result).Returns(
                bankAccount);
            await _bankAccountService.UpdateBankAccount(bankAccount);
            _fakeBankAccountRepository.Verify(repository => repository.UpdateBankAccount(bankAccount), Times.Once);
        }

        [Fact]
        public async Task UpdateBankAccount_CheckValidatorWork_ShouldCheckValidatorCall()
        {
            const int id = 3;
            const string currency = "RUB";
            _fakeBankAccountRepository.Setup(repository => repository.GetAccount(id).Result).
                Returns(new BankAccount {Id = id, Currency = currency});
            await _bankAccountService.UpdateBankAccount(new BankAccount
                 {
                     Id = id,
                     Balance = 1000, Currency = currency, IsOpen = true,
                     OpenAccountDate = DateTime.Now, CloseAccountDate = DateTime.MinValue
                 });
            Assert.True(_bankAccountValidator.IsCalled);
        }

        [Fact]
        public async Task UpdateBankAccount_NonExistentAccount_ShouldThrowValidationException()
        {
            const int bankAccountId = 2, balance = 1000;
            const string currency = "RUB";
            _fakeBankAccountRepository.Setup(repository => repository.GetAccount(bankAccountId).Result).
                Throws(new ValidationException(BankAccountMessages.NonExistentAccount));
            var exception = await Assert.ThrowsAsync<ValidationException>(() =>
                _bankAccountService.UpdateBankAccount(new BankAccount{Id = bankAccountId, Currency = currency, Balance = balance}));
            Assert.Equal(BankAccountMessages.NonExistentAccount, exception.Message);
        }
        
        [Fact]
        public async Task GetBankAccount_NonExistentAccount_ShouldThrowValidationException()
        {
            const int id = 4;
            _fakeBankAccountRepository.Setup(repository => repository.GetAccount(id).Result).
                Throws(new ValidationException(BankAccountMessages.NonExistentAccount));
            var exception = await Assert.ThrowsAsync<ValidationException>(() => _bankAccountService.GetAccount(id)); 
            Assert.Equal(BankAccountMessages.NonExistentAccount, exception.Message);
        }
        
        [Fact]
        public async Task GetBankAccount_SuccessPath_ShouldGetBankAccountFromBase()
        {
            const int bankAccountId = 3, userId = 2, balance = 1000;
            const string currency = "RUB";
            var date = DateTime.MinValue; 
            _fakeBankAccountRepository.Setup(repository => repository.GetAccount(bankAccountId).Result).Returns(new 
                BankAccount{Balance = balance, Currency = currency, IsOpen = true, UserId = userId, 
                    Id = bankAccountId, CloseAccountDate = date, 
                    OpenAccountDate = date});
            var result = await _bankAccountService.GetAccount(bankAccountId);
            Assert.True((int) result.Balance == balance);
            Assert.True(result.Currency == currency);
            Assert.True(result.IsOpen);
            Assert.True(result.UserId == userId);
            Assert.True(result.Id == bankAccountId);
            Assert.True(result.CloseAccountDate == date);
            Assert.True(result.OpenAccountDate == date);
        }
        

        [Theory]
        [InlineData(-100,4,5)]
        [InlineData(0,4,5)]
        public async Task GetCommission_WithIncorrectValue_ShouldThrowValidationException(int value, int fromAccountId, int toAccountId)
        {
            var exception = await Assert.ThrowsAsync<ValidationException>(() => 
                _bankAccountService.GetCommission(value, fromAccountId, toAccountId));
            Assert.Equal(BankAccountMessages.ZeroOrNegativeValue, exception.Message);
        }
        
        [Fact]
        public async Task GetCommission_ForNonExistentAccounts_ShouldThrowValidationException()
        {
            const int userId1 = 4, userId2 = 5, balance = 1000;
            _fakeBankAccountRepository.Setup(repository => repository.GetAccount(userId1).Result)
                .Throws(new ValidationException(BankAccountMessages.NonExistentAccount));
            _fakeBankAccountRepository.Setup(repository => repository.GetAccount(userId2).Result)
                .Returns(new BankAccount{});
            var exception = await Assert.ThrowsAsync<ValidationException>(() => 
                _bankAccountService.GetCommission(balance, userId1, userId2));
            Assert.Equal(BankAccountMessages.NonExistentAccount, exception.Message);
            
            _fakeBankAccountRepository.Setup(repository => repository.GetAccount(userId1).Result)
                .Returns(new BankAccount{});
            _fakeBankAccountRepository.Setup(repository => repository.GetAccount(userId2).Result)
                .Throws(new ValidationException(BankAccountMessages.NonExistentAccount));
            exception = await Assert.ThrowsAsync<ValidationException>(() => 
                _bankAccountService.GetCommission(balance, userId1, userId2));
            Assert.Equal(BankAccountMessages.NonExistentAccount, exception.Message);
            
            
            _fakeBankAccountRepository.Setup(repository => repository.GetAccount(userId1).Result)
                .Throws(new ValidationException(BankAccountMessages.NonExistentAccount));
            _fakeBankAccountRepository.Setup(repository => repository.GetAccount(userId2).Result)
                .Throws(new ValidationException(BankAccountMessages.NonExistentAccount));
            exception = await Assert.ThrowsAsync<ValidationException>(() => 
                _bankAccountService.GetCommission(balance, userId1, userId2));
            Assert.Equal(BankAccountMessages.NonExistentAccount, exception.Message);
        }

        [Fact]
        public async Task GetCommission_ForAccountsWhichAreClosed_ShouldThrowValidationException()
        {
            const int userId1 = 4, userId2 = 5, balance = 1000;
            _fakeBankAccountRepository.Setup(repository => repository.GetAccount(userId1).Result)
                .Returns(new BankAccount{IsOpen = false});
            _fakeBankAccountRepository.Setup(repository => repository.GetAccount(userId2).Result)
                .Returns(new BankAccount{IsOpen = true});
            var exception = await Assert.ThrowsAsync<ValidationException>(() => 
                _bankAccountService.GetCommission(balance, userId1, userId2));
            Assert.Equal(BankAccountMessages.GetCommissionForClosedAccount, exception.Message);
            
            _fakeBankAccountRepository.Setup(repository => repository.GetAccount(userId1).Result)
                .Returns(new BankAccount{IsOpen = true});
            _fakeBankAccountRepository.Setup(repository => repository.GetAccount(userId2).Result)
                .Returns(new BankAccount{IsOpen = false});
            exception = await Assert.ThrowsAsync<ValidationException>(() => 
                _bankAccountService.GetCommission(balance, userId1, userId2));
            Assert.Equal(BankAccountMessages.GetCommissionForClosedAccount, exception.Message);
            
            _fakeBankAccountRepository.Setup(repository => repository.GetAccount(userId1).Result)
                .Returns(new BankAccount{IsOpen = false});
            _fakeBankAccountRepository.Setup(repository => repository.GetAccount(userId2).Result)
                .Returns(new BankAccount{IsOpen = false});
            exception = await Assert.ThrowsAsync<ValidationException>(() => 
                _bankAccountService.GetCommission(balance, userId1, userId2));
            Assert.Equal(BankAccountMessages.GetCommissionForClosedAccount, exception.Message);
        }

        [Fact]
        public async Task GetCommission_WithIncorrectCommission_ShouldCorrect()
        {
            const int userId1 = 20, userId2 = 19, bankAccountId1 = 4, bankAccountId2 = 5, bankAccountId3 = 6;
            _fakeBankAccountRepository.Setup(repository => repository.GetAccount(bankAccountId1).Result)
                .Returns(new BankAccount{UserId = userId1, IsOpen = true});
            _fakeBankAccountRepository.Setup(repository => repository.GetAccount(bankAccountId2).Result)
                .Returns(new BankAccount{UserId = userId1, IsOpen = true});
            _fakeBankAccountRepository.Setup(repository => repository.GetAccount(bankAccountId3).Result)
                .Returns(new BankAccount{UserId = userId2, IsOpen = true});
            var result1 = await _bankAccountService.GetCommission(1000, bankAccountId1, bankAccountId2);
            var result2 = await _bankAccountService.GetCommission(1000, bankAccountId1, bankAccountId3);
            var result3 = await _bankAccountService.GetCommission(123456, bankAccountId1, bankAccountId3);
            Assert.True(result1 == 0);
            Assert.True((int) result2 == 20); 
            Assert.True((int) result3 == 2469);
        }
        
        [Fact]
        public async Task CloseAccount_NonExistentAccount_ShouldThrowValidationException()
        {
            const int bankAccountId = 4;
            _fakeBankAccountRepository.Setup(repository => repository.GetAccount(bankAccountId).Result)
                .Throws(new ValidationException(BankAccountMessages.NonExistentAccount));
            var exception = await Assert.ThrowsAsync<ValidationException>(() => _bankAccountService.CloseAccount(bankAccountId));
            Assert.Equal(BankAccountMessages.NonExistentAccount, exception.Message);
        }

        [Fact]
        public async Task CloseAccount_WithNotZeroBalance_ShouldThrowValidationException()
        {
            const int bankAccountId = 4;
            _fakeBankAccountRepository.Setup(repository => repository.GetAccount(bankAccountId).Result)
                .Returns(new BankAccount{Balance = 1000});
            var exception = await Assert.ThrowsAsync<ValidationException>(() => _bankAccountService.CloseAccount(bankAccountId));
            Assert.Equal(BankAccountMessages.CloseAccountWithNotZeroBalance, exception.Message);
        }

        [Fact]
        public async Task CloseAccount_WhichIsAlreadyClosed_ShouldThrowValidationException()
        {
            const int bankAccountId = 4;
            _fakeBankAccountRepository.Setup(repository => repository.GetAccount(bankAccountId).Result)
                .Returns(new BankAccount{Balance = 0, IsOpen = false});
            var exception = await Assert.ThrowsAsync<ValidationException>(() => _bankAccountService.CloseAccount(bankAccountId));
            Assert.Equal(BankAccountMessages.CloseAccountThatAreAlreadyClosed, exception.Message);
        }

        [Fact]
        public async Task CloseAccount_SuccessPath_ShouldCheckAccountChanges()
        {
            const int bankAccountId = 4;
            _fakeBankAccountRepository.Setup(repository => repository.GetAccount(bankAccountId).Result).
                Returns(new BankAccount{Balance = 0, IsOpen = true});
            await _bankAccountService.CloseAccount(bankAccountId);
            var result = await _bankAccountService.GetAccount(bankAccountId);
            Assert.True(!result.IsOpen); 
            Assert.NotNull(result.CloseAccountDate);
            Assert.True(result.CloseAccountDate != DateTime.MinValue);
        }

        [Fact]
        public async Task MakeMoneyTransfer_ToTheSameAccount_ShouldThrowValidationException()
        {
            var exception = await Assert.ThrowsAsync<ValidationException>(() => 
                _bankAccountService.MakeMoneyTransfer(100, 4, 4));
            Assert.Equal(BankAccountMessages.TransferMoneyToTheSameAccount, exception.Message);
        }

        [Theory]
        [InlineData(-100,4,5)]
        [InlineData(0,4,5)]
        public async Task MakeMoneyTransfer_ValueLessThanZeroOrEqualToZero_ShouldThrowValidationException(int value, int fromAccountId, int toAccountId)
        {
            var exception = await Assert.ThrowsAsync<ValidationException>(() => 
                _bankAccountService.MakeMoneyTransfer(value, fromAccountId, toAccountId));
            Assert.Equal(BankAccountMessages.ZeroOrNegativeValue, exception.Message);
        }
        
        [Fact]
        public async Task MakeMoneyTransfer_ForNonExistentAccounts_ShouldThrowValidationException()
        {
            const int userId1 = 4, userId2 = 5, balance = 1000;
            _fakeBankAccountRepository.Setup(repository => repository.GetAccount(userId1).Result)
                .Throws(new ValidationException(BankAccountMessages.NonExistentAccount));
            _fakeBankAccountRepository.Setup(repository => repository.GetAccount(userId2).Result)
                .Returns(new BankAccount{});
            var exception = await Assert.ThrowsAsync<ValidationException>(() => 
                _bankAccountService.MakeMoneyTransfer(balance, userId1, userId2));
            Assert.Equal(BankAccountMessages.NonExistentAccount, exception.Message);
            
            _fakeBankAccountRepository.Setup(repository => repository.GetAccount(userId1).Result)
                .Returns(new BankAccount{});
            _fakeBankAccountRepository.Setup(repository => repository.GetAccount(userId2).Result)
                .Throws(new ValidationException(BankAccountMessages.NonExistentAccount));
            exception = await Assert.ThrowsAsync<ValidationException>(() => 
                _bankAccountService.MakeMoneyTransfer(balance, userId1, userId2));
            Assert.Equal(BankAccountMessages.NonExistentAccount, exception.Message);
            
            _fakeBankAccountRepository.Setup(repository => repository.GetAccount(userId1).Result)
                .Throws(new ValidationException(BankAccountMessages.NonExistentAccount));
            _fakeBankAccountRepository.Setup(repository => repository.GetAccount(userId2).Result)
                .Throws(new ValidationException(BankAccountMessages.NonExistentAccount));
            exception = await Assert.ThrowsAsync<ValidationException>(() => 
                _bankAccountService.MakeMoneyTransfer(balance, userId1, userId2));
            Assert.Equal(BankAccountMessages.NonExistentAccount, exception.Message);
        }

        [Fact]
        public async Task MakeMoneyTransfer_WithMissingAmount_ShouldThrowValidationException()
        {
            const int bankAccountId = 4;
            _fakeBankAccountRepository.Setup(repository => repository.GetAccount(bankAccountId).Result)
                .Returns(new BankAccount {Balance = 1000});
            var exception = await Assert.ThrowsAsync<ValidationException>(() => 
                _bankAccountService.MakeMoneyTransfer(100000, bankAccountId, 5));   
            Assert.Equal(BankAccountMessages.NotEnoughBalance, exception.Message);
        }
        
        [Fact]
        public async Task MakeMoneyTransfer_ForAccountsWhichAreClosed_ShouldThrowValidationException()
        {
            const int userId1 = 4, userId2 = 5, moneyTransfer = 1000, balance = 100000;
            _fakeBankAccountRepository.Setup(repository => repository.GetAccount(userId1).Result)
                .Returns(new BankAccount{IsOpen = false, Balance = balance});
            _fakeBankAccountRepository.Setup(repository => repository.GetAccount(userId2).Result)
                .Returns(new BankAccount{IsOpen = true, Balance = balance});
            var exception = await Assert.ThrowsAsync<ValidationException>(() => 
                _bankAccountService.MakeMoneyTransfer(moneyTransfer, userId1, userId2));
            Assert.Equal(BankAccountMessages.MoneyTransferBetweenClosedAccounts, exception.Message);
            
            _fakeBankAccountRepository.Setup(repository => repository.GetAccount(userId1).Result)
                .Returns(new BankAccount{IsOpen = true, Balance = balance});
            _fakeBankAccountRepository.Setup(repository => repository.GetAccount(userId2).Result)
                .Returns(new BankAccount{IsOpen = false, Balance = balance});
            exception = await Assert.ThrowsAsync<ValidationException>(() => 
                _bankAccountService.MakeMoneyTransfer(moneyTransfer, userId1, userId2));
            Assert.Equal(BankAccountMessages.MoneyTransferBetweenClosedAccounts, exception.Message);
            
            _fakeBankAccountRepository.Setup(repository => repository.GetAccount(userId1).Result)
                .Returns(new BankAccount{IsOpen = false, Balance = balance});
            _fakeBankAccountRepository.Setup(repository => repository.GetAccount(userId2).Result)
                .Returns(new BankAccount{IsOpen = false, Balance = balance});
            await Assert.ThrowsAsync<ValidationException>(() => 
                _bankAccountService.MakeMoneyTransfer(moneyTransfer, userId1, userId2));
            Assert.Equal(BankAccountMessages.MoneyTransferBetweenClosedAccounts, exception.Message);
        }

        [Fact]
        public async Task MakeMoneyTransfer_SuccessPath_ShouldCheckAccountsChanges()
        {
            const int balance = 100000, bankAccountId1 = 4, bankAccountId2 = 5, bankAccountId3 = 6, userId1 = 1, userId2 = 2, moneyTransfer = 20000;
            const string currency = "Rub";
            _fakeBankAccountRepository.Setup(repository => repository.GetAccount(bankAccountId1).Result).Returns(new BankAccount
                {IsOpen = true, Balance = balance, Currency = currency, UserId = userId1, Id = bankAccountId1});
            _fakeBankAccountRepository.Setup(repository => repository.GetAccount(bankAccountId2).Result).Returns(new BankAccount
                {IsOpen = true, Balance = balance, Currency = currency, UserId = userId1, Id = bankAccountId2});
            _fakeBankAccountRepository.Setup(repository => repository.GetAccount(bankAccountId3).Result).Returns(new BankAccount
                {IsOpen = true, Balance = balance, Currency = currency, UserId = userId2, Id = bankAccountId3});
            _fakeCurrencyConverter.Setup(converter => converter.GetValueInOtherCurrency(moneyTransfer, currency, currency).Result).Returns(20000);
            _fakeCurrencyConverter.Setup(converter => converter.GetValueInOtherCurrency(0, currency, currency).Result).Returns(0);
            _fakeCurrencyConverter.Setup(converter => converter.GetValueInOtherCurrency(400, currency, currency).Result).Returns(400);
            await _bankAccountService.MakeMoneyTransfer(moneyTransfer, bankAccountId1, bankAccountId2);
            var account1 = await _bankAccountService.GetAccount(bankAccountId1);
            var account2 = await _bankAccountService.GetAccount(bankAccountId2);
            Assert.True((int) account1.Balance == 80000);
            Assert.True((int) account2.Balance == 120000);
            Assert.True(account1.IsOpen && account2.IsOpen);
            await _bankAccountService.MakeMoneyTransfer(moneyTransfer, bankAccountId1, bankAccountId3);
            account1 = await _bankAccountService.GetAccount(bankAccountId1);
            account2 = await _bankAccountService.GetAccount(bankAccountId3);
            Assert.True((int) account1.Balance == 60000);
            Assert.True((int) account2.Balance == 119600); 
            Assert.True(account1.IsOpen && account2.IsOpen);
        }
    }
}