using System;
using System.Threading.Tasks;
using FluentValidation;
using Minibank.Core.Domains.BankAccounts;
using Minibank.Core.Domains.BankAccounts.Repositories;
using Minibank.Core.Domains.BankAccounts.Services;
using Minibank.Core.Domains.MoneyTransferHistory.Services;
using Minibank.Core.Domains.Users.Repositories;
using Moq;
using Xunit;

namespace Minibank.Core.Tests
{
    public class BankAccountServiceTests
    {
        private readonly Mock<IBankAccountRepository> _fakeBankAccountRepository;
        private readonly Mock<IUserRepository> _fakeUserRepository;
        private readonly IBankAccountService _bankAccountService;

        public BankAccountServiceTests()
        {
            _fakeUserRepository = new Mock<IUserRepository>();
            _fakeBankAccountRepository = new Mock<IBankAccountRepository>();
            _bankAccountService = new BankAccountService(_fakeBankAccountRepository.Object, _fakeUserRepository.Object,
                new Mock<ICurrencyConverter>().Object,
                new Mock<IMoneyTransferHistoryService>().Object, new Mock<IUnitOfWork>().Object, new Mock<IValidator<BankAccount>>().Object);
        }
        
        

        [Fact]
        public async Task GetCommission_WithIncorrectValue_ShouldThrowValidationException()
        {
            await Assert.ThrowsAsync<ValidationException>(() => _bankAccountService.GetCommission(-100, 4, 5));
            await Assert.ThrowsAsync<ValidationException>(() => _bankAccountService.GetCommission(0, 4, 5));
        }
        
        [Fact]
        public async Task GetCommission_ForAccountsWhichAreNotInBase_ShouldThrowValidationException()
        {
            _fakeBankAccountRepository.Setup(repository => repository.GetAccount(4).Result)
                .Throws<Exception>();
            _fakeBankAccountRepository.Setup(repository => repository.GetAccount(5).Result)
                .Returns(new BankAccount{});
            await Assert.ThrowsAsync<Exception>(() => _bankAccountService.GetCommission(1000, 4, 5));
            
            _fakeBankAccountRepository.Setup(repository => repository.GetAccount(4).Result)
                .Returns(new BankAccount{});
            _fakeBankAccountRepository.Setup(repository => repository.GetAccount(5).Result)
                .Throws<Exception>();
            await Assert.ThrowsAsync<Exception>(() => _bankAccountService.GetCommission(1000, 4, 5));
            
            _fakeBankAccountRepository.Setup(repository => repository.GetAccount(4).Result)
                .Throws<Exception>();
            _fakeBankAccountRepository.Setup(repository => repository.GetAccount(5).Result)
                .Throws<Exception>();
            await Assert.ThrowsAsync<Exception>(() => _bankAccountService.GetCommission(1000, 4, 5));
        }

        [Fact]
        public async Task GetCommission_ForAccountsWhichAreClosed_ShouldThrowValidationException()
        {
            _fakeBankAccountRepository.Setup(repository => repository.GetAccount(4).Result)
                .Returns(new BankAccount{IsOpen = false});
            _fakeBankAccountRepository.Setup(repository => repository.GetAccount(5).Result)
                .Returns(new BankAccount{IsOpen = true});
            await Assert.ThrowsAsync<ValidationException>(() => _bankAccountService.GetCommission(1000, 4, 5));
            
            _fakeBankAccountRepository.Setup(repository => repository.GetAccount(4).Result)
                .Returns(new BankAccount{IsOpen = true});
            _fakeBankAccountRepository.Setup(repository => repository.GetAccount(5).Result)
                .Returns(new BankAccount{IsOpen = false});
            await Assert.ThrowsAsync<ValidationException>(() => _bankAccountService.GetCommission(1000, 4, 5));
            
            _fakeBankAccountRepository.Setup(repository => repository.GetAccount(4).Result)
                .Returns(new BankAccount{IsOpen = false});
            _fakeBankAccountRepository.Setup(repository => repository.GetAccount(5).Result)
                .Returns(new BankAccount{IsOpen = false});
            await Assert.ThrowsAsync<ValidationException>(() => _bankAccountService.GetCommission(1000, 4, 5));
        }

        [Fact]
        public async Task GetCommission_WithIncorrectCommission_ShouldCorrect()
        {
            _fakeBankAccountRepository.Setup(repository => repository.GetAccount(4).Result)
                .Returns(new BankAccount{UserId = 20, IsOpen = true});
            _fakeBankAccountRepository.Setup(repository => repository.GetAccount(5).Result)
                .Returns(new BankAccount{UserId = 20, IsOpen = true});
            _fakeBankAccountRepository.Setup(repository => repository.GetAccount(6).Result)
                .Returns(new BankAccount{UserId = 19, IsOpen = true});
            var result1 = await _bankAccountService.GetCommission(1000, 4, 5);
            var result2 = await _bankAccountService.GetCommission(1000, 4, 6);
            var result3 = await _bankAccountService.GetCommission(123456, 4, 6);
            Assert.True(result1 == 0 && (int) result2 == 20 && (int) result3 == 2469);
        }
        
        [Fact]
        public async Task CloseAccount_AccountWhichIsNotInBase_ShouldThrowValidationException()
        {
            _fakeBankAccountRepository.Setup(repository => repository.GetAccount(4).Result)
                .Throws<Exception>();
            await Assert.ThrowsAsync<Exception>(() => _bankAccountService.CloseAccount(4));
        }

        [Fact]
        public async Task CloseAccount_WithNotZeroBalance_ShouldThrowValidationException()
        {
            _fakeBankAccountRepository.Setup(repository => repository.GetAccount(4).Result)
                .Returns(new BankAccount{Balance = 1000});
            await Assert.ThrowsAsync<ValidationException>(() => _bankAccountService.CloseAccount(4));
        }

        [Fact]
        public async Task CloseAccount_WhichIsAlreadyClosed_ShouldThrowValidationException()
        {
            _fakeBankAccountRepository.Setup(repository => repository.GetAccount(4).Result)
                .Returns(new BankAccount{Balance = 0, IsOpen = false});
            await Assert.ThrowsAsync<ValidationException>(() => _bankAccountService.CloseAccount(4));
        }

        [Fact]
        public async Task CloseAccount_SuccessPath_ShouldCheckAccountChanges()
        {
            _fakeBankAccountRepository.Setup(repository => repository.GetAccount(4).Result).
                Returns(new BankAccount{Balance = 0, IsOpen = true});
            await _bankAccountService.CloseAccount(4);
            var result = await _bankAccountService.GetAccount(4);
            Assert.True(!result.IsOpen && result.CloseAccountDate != DateTime.MinValue);
        }

        [Fact]
        public async Task MakeMoneyTransfer_WithIncorrectValue_ShouldThrowValidationException()
        {
            await Assert.ThrowsAsync<ValidationException>(() => _bankAccountService.MakeMoneyTransfer(-100, 4, 5));   
            await Assert.ThrowsAsync<ValidationException>(() => _bankAccountService.MakeMoneyTransfer(0, 4, 5)); 
        }
        
        [Fact]
        public async Task MakeMoneyTransfer_ForAccountsWhichAreNotInBase_ShouldThrowValidationException()
        {
            _fakeBankAccountRepository.Setup(repository => repository.GetAccount(4).Result)
                .Throws<Exception>();
            _fakeBankAccountRepository.Setup(repository => repository.GetAccount(5).Result)
                .Returns(new BankAccount{});
            await Assert.ThrowsAsync<Exception>(() => _bankAccountService.MakeMoneyTransfer(1000, 4, 5));
            
            _fakeBankAccountRepository.Setup(repository => repository.GetAccount(4).Result)
                .Returns(new BankAccount{});
            _fakeBankAccountRepository.Setup(repository => repository.GetAccount(5).Result)
                .Throws<Exception>();
            await Assert.ThrowsAsync<Exception>(() => _bankAccountService.MakeMoneyTransfer(1000, 4, 5));
            
            _fakeBankAccountRepository.Setup(repository => repository.GetAccount(4).Result)
                .Throws<Exception>();
            _fakeBankAccountRepository.Setup(repository => repository.GetAccount(5).Result)
                .Throws<Exception>();
            await Assert.ThrowsAsync<Exception>(() => _bankAccountService.MakeMoneyTransfer(1000, 4, 5));
        }

        [Fact]
        public async Task MakeMoneyTransfer_WithMissingAmount_ShouldThrowValidationException()
        {
            _fakeBankAccountRepository.Setup(repository => repository.GetAccount(4).Result)
                .Returns(new BankAccount {Balance = 1000});
            await Assert.ThrowsAsync<ValidationException>(() => _bankAccountService.MakeMoneyTransfer(100000, 4, 5));
        }
        
        [Fact]
        public async Task MakeMoneyTransfer_ForAccountsWhichAreClosed_ShouldThrowValidationException()
        {
            _fakeBankAccountRepository.Setup(repository => repository.GetAccount(4).Result)
                .Returns(new BankAccount{IsOpen = false});
            _fakeBankAccountRepository.Setup(repository => repository.GetAccount(5).Result)
                .Returns(new BankAccount{IsOpen = true});
            await Assert.ThrowsAsync<ValidationException>(() => _bankAccountService.MakeMoneyTransfer(1000, 4, 5));
            
            _fakeBankAccountRepository.Setup(repository => repository.GetAccount(4).Result)
                .Returns(new BankAccount{IsOpen = true});
            _fakeBankAccountRepository.Setup(repository => repository.GetAccount(5).Result)
                .Returns(new BankAccount{IsOpen = false});
            await Assert.ThrowsAsync<ValidationException>(() => _bankAccountService.MakeMoneyTransfer(1000, 4, 5));
            
            _fakeBankAccountRepository.Setup(repository => repository.GetAccount(4).Result)
                .Returns(new BankAccount{IsOpen = false});
            _fakeBankAccountRepository.Setup(repository => repository.GetAccount(5).Result)
                .Returns(new BankAccount{IsOpen = false});
            await Assert.ThrowsAsync<ValidationException>(() => _bankAccountService.MakeMoneyTransfer(1000, 4, 5));
        }

        [Fact]
        public async Task MakeMoneyTransfer_SuccessPath_ShouldCheckAccountsChanges()
        {
            _fakeBankAccountRepository.Setup(repository => repository.GetAccount(4).Result).Returns(new BankAccount
                {IsOpen = true, Balance = 100000, Currency = "Rub", UserId = 1, Id = 4});
            _fakeBankAccountRepository.Setup(repository => repository.GetAccount(5).Result).Returns(new BankAccount
                {IsOpen = true, Balance = 100000, Currency = "Rub", UserId = 1, Id = 5});
            _fakeBankAccountRepository.Setup(repository => repository.GetAccount(6).Result).Returns(new BankAccount
                {IsOpen = true, Balance = 100000, Currency = "Rub", UserId = 2, Id = 6});
            await _bankAccountService.MakeMoneyTransfer(20000, 4, 5);
            var account1 = await _bankAccountService.GetAccount(4);
            var account2 = await _bankAccountService.GetAccount(5);
            Assert.True((int) account1.Balance == 80000 && (int) account2.Balance == 120000 && account1.IsOpen && account2.IsOpen);
            await _bankAccountService.MakeMoneyTransfer(20000, 4, 6);
            account1 = await _bankAccountService.GetAccount(4);
            account2 = await _bankAccountService.GetAccount(6);
            Assert.True((int) account1.Balance == 60000 && (int) account2.Balance == 139600 && account1.IsOpen && account2.IsOpen);
        }
    }
}