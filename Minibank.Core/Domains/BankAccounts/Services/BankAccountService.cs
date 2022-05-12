using System;
using System.Threading.Tasks;
using FluentValidation;
using Minibank.Core.Domains.BankAccounts.Repositories;
using Minibank.Core.Domains.MoneyTransferHistory.Services;
using Minibank.Core.Domains.Users.Repositories;
using Minibank.Core.Enums;

namespace Minibank.Core.Domains.BankAccounts.Services
{
    public class BankAccountService : IBankAccountService
    {
        private readonly IBankAccountRepository _bankAccountRepository;
        private readonly IUserRepository _userRepository;
        
        private readonly IMoneyTransferHistoryService _moneyTransferHistory;

        private readonly ICurrencyConverter _converter;

        private readonly IUnitOfWork _unitOfWork;

        private readonly IValidator<BankAccount> _bankAccountValidator;
        public BankAccountService(IBankAccountRepository bankAccountRepository, IUserRepository userRepository, 
            ICurrencyConverter converter, IMoneyTransferHistoryService moneyTransferHistory, IUnitOfWork unitOfWork, IValidator<BankAccount> bankAccountValidator)
        {
            _bankAccountRepository = bankAccountRepository;
            _userRepository = userRepository;
            _converter = converter;
            _moneyTransferHistory = moneyTransferHistory;
            _unitOfWork = unitOfWork;
            _bankAccountValidator = bankAccountValidator;
        }
        
        

        public async Task CreateBankAccount(int userId, string currencyCode, double startBalance)
        {
            await _bankAccountValidator.ValidateAndThrowAsync(new BankAccount
                {UserId = userId, Currency = currencyCode, Balance = startBalance});
            await _userRepository.GetUser(userId);
            await _bankAccountRepository.CreateBankAccount(userId, currencyCode, startBalance);
            await _unitOfWork.SaveChangesAsync();
        }
        
        public async Task<BankAccount> GetAccount(int id)
        {
            return await _bankAccountRepository.GetAccount(id);
        }

        public async Task UpdateBankAccount(BankAccount bankAccount)
        { 
            await _bankAccountValidator.ValidateAndThrowAsync(bankAccount);
            await _userRepository.GetUser(bankAccount.UserId);
           await _bankAccountRepository.UpdateBankAccount(bankAccount);
           await _unitOfWork.SaveChangesAsync();
        }

        public async Task CloseAccount(int id)
        {
            var entity = await _bankAccountRepository.GetAccount(id);
            if (entity.Balance != 0)
            {
                throw new ValidationException("Before closing BankAccount your balance must be 0!");
            }
            if (!entity.IsOpen)
            {
                throw new ValidationException("This account has already closed!");
            }
            entity.IsOpen = false;
            entity.CloseAccountDate = DateTime.Now;
            await _bankAccountRepository.UpdateBankAccount(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        private async Task<double> GetCommissionValue(double value, int userId1, int userId2)
        {
            return await Task.Run(() =>
            {
                double number = userId1 == userId2 ? 0 : Math.Floor(Math.Round(value * 0.02, 2));
                return number;
            });
        }
        
        public async Task<double> GetCommission(double value, int fromAccountId, int toAccountId)
        {
            if (value <= 0)
            {
                throw new ValidationException("value must be more, than 0!");
            }
            var bankAccount1 = await _bankAccountRepository.GetAccount(fromAccountId);
            var bankAccount2 = await _bankAccountRepository.GetAccount(toAccountId);
            if (!bankAccount1.IsOpen || !bankAccount2.IsOpen)
            {
                throw new ValidationException("You can't get commission, because one of this accounts is closed!");
            }
            return await GetCommissionValue(value, bankAccount1.UserId, bankAccount2.UserId);
        }
        
        public async Task MakeMoneyTransfer(double value, int fromAccountId, int toAccountId)
        {
            if (fromAccountId == toAccountId)
            {
                throw new ValidationException("You can't transfer money to the same account!");
            }
            if (value <= 0)
            {
                throw new ValidationException("value must be more, than 0!");
            }
            var bankAccount1 = await _bankAccountRepository.GetAccount(fromAccountId);
            var bankAccount2 = await _bankAccountRepository.GetAccount(toAccountId);
            if (bankAccount1.Balance < value)
            {
                throw new ValidationException("You don't have enough funds on your balance!");
            }
            if (!bankAccount1.IsOpen || !bankAccount2.IsOpen)
            {
                throw new ValidationException("You can't get commission, because one of this accounts is closed!");
            }
            double commission = await _converter.GetValueInOtherCurrency(await GetCommissionValue(value, bankAccount1.UserId, bankAccount2.UserId), 
                bankAccount1.Currency, bankAccount2.Currency);
            var valueTo =  await _converter.GetValueInOtherCurrency(value, bankAccount1.Currency, bankAccount2.Currency) - commission;
            await _moneyTransferHistory.AddHistory(value, bankAccount1.Currency, bankAccount1.Id, bankAccount2.Id);
            bankAccount1.Balance -= value;
            bankAccount2.Balance += valueTo;
            await _bankAccountRepository.UpdateBankAccount(bankAccount1);
            await _bankAccountRepository.UpdateBankAccount(bankAccount2);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}