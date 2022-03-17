using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Minibank.Core;
using Minibank.Core.Domains.BankAccounts;
using Minibank.Core.Domains.BankAccounts.Repositories;
using Minibank.Core.Domains.MoneyTransferHistory.Services;
using Minibank.Core.Domains.Users.Repositories;

namespace Minibank.Data.BankAccounts.Repositories
{
    public class BankAccountRepository : IBankAccountRepository
    {
        private static List<BankAccountDBModel> _bankAccountDbModels = new List<BankAccountDBModel>();
        
        private readonly IUserRepository _userRepository;
        
        private readonly ICurrencyConverter _converter;

        private readonly IMoneyTransferHistoryService _moneyTransferHistory;
        
        public BankAccountRepository(IUserRepository userRepository, ICurrencyConverter converter, IMoneyTransferHistoryService moneyTransferHistory)
        {
            _userRepository = userRepository;
            _converter = converter;
            _moneyTransferHistory = moneyTransferHistory;
        }

        public void CreateBankAccount(string userId, string currencyCode, double startBalance)
        {
            // if (startBalance < 0) throw new ValidationException("StartBalance must be more than 0 or equal 0!");
            // if (currency == null || (currency.ToUpper() != "RUB" && currency.ToUpper() != "USD" &&
            //                          currency.ToUpper() != "EUR"))
            //     throw new ValidationException("This currency is unavailable at the moment!");
            //
            // var user = _userRepository.Get(userId);
            // if (user.HasBankAccount)
            //     throw new ValidationException("This user has a BankAccount! User mustn't have more BankAccounts!");
            // user.HasBankAccount = true;
            var entity = new BankAccountDBModel
            {
                Id = Guid.NewGuid().ToString(),
                UserId = userId,
                Balance = startBalance,
                Currency = currencyCode,
                IsOpen = true,
                OpenAccountDate = DateTime.Now,
                CloseAccountDate = DateTime.MinValue
            };
            _bankAccountDbModels.Add(entity);
        }

        public BankAccount GetAccount(string accountId)
        {
            var entity = _bankAccountDbModels.FirstOrDefault(it => it.Id == accountId);
            if (entity == null) throw new ValidationException("This accountId is not found in base!");
            return new BankAccount
            {
                Id = entity.Id,
                UserId = entity.UserId,
                Balance = entity.Balance,
                Currency = entity.Currency,
                IsOpen = entity.IsOpen,
                OpenAccountDate = entity.OpenAccountDate,
                CloseAccountDate = entity.CloseAccountDate
            };
        }

        public double GetCommision(double value, string fromAccountId, string toAccountId)
        {
            var bankAccount1 = _bankAccountDbModels.FirstOrDefault(it => it.Id == fromAccountId);
            var bankAccount2 = _bankAccountDbModels.FirstOrDefault(it => it.Id == toAccountId);
            return bankAccount1.UserId == bankAccount2.UserId ? 0 : Math.Floor(Math.Round(value * 0.02, 2));

            // if (bankAccount1.UserId == bankAccount2.UserId) return 0;
            // var result = Math.Round(value * 0.02, 2);
            // result = Math.Floor(result);
            // return result; 
        }

        public void MakeMoneyTransfer(double value, string fromAccountId, string toAccountId)
        {
            var bankAccount1 = _bankAccountDbModels.FirstOrDefault(it => it.Id == fromAccountId);
            var bankAccount2 = _bankAccountDbModels.FirstOrDefault(it => it.Id == toAccountId);
            double commision = _converter.GetValueInOtherCurrency(GetCommision(value, fromAccountId, toAccountId), 
                bankAccount1.Currency, bankAccount2.Currency);
            if (bankAccount1.Balance < value)
                throw new ValidationException("You don't have enough funds on your balance!");
            bankAccount1.Balance -= value;
            bankAccount2.Balance += 
                _converter.GetValueInOtherCurrency(value, bankAccount1.Currency, bankAccount2.Currency) - commision;
            _moneyTransferHistory.AddHistory(value, bankAccount1.Currency, fromAccountId, toAccountId);
        }
        
        public void CloseAccount(string id)
        {
            var entity = _bankAccountDbModels.FirstOrDefault(it => it.Id == id);
            if (entity.Balance != 0)
                throw new ValidationException("Before closing BankAccount your balance must be 0!");
            if (!entity.IsOpen) throw new ValidationException("This account has already closed!");
            entity.IsOpen = false;
            entity.CloseAccountDate = DateTime.Now;
        }


        // public void CloseAccount(string accountId)
        // {
        //     var entity = _bankAccountDbModels.FirstOrDefault(it => it.Id == accountId);
        //     if (entity == null) throw new ValidationException("This accountId is not found in base!");
        //     if (entity.Balance != 0)
        //         throw new ValidationException("Before closing BankAccount your balance must be 0!");
        //     entity.IsOpen = false;
        // }
    }
}