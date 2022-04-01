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

        private readonly IMoneyTransferHistoryService _moneyTransferHistory;

        private static int _id = 1;
        public BankAccountRepository(IUserRepository userRepository, ICurrencyConverter converter, IMoneyTransferHistoryService moneyTransferHistory)
        {
            _userRepository = userRepository;
            _moneyTransferHistory = moneyTransferHistory;
        }

        public void CreateBankAccount(int userId, string currencyCode, double startBalance)
        {
            var entity = new BankAccountDBModel
            {
                Id = _id++,
                UserId = userId,
                Balance = startBalance,
                Currency = currencyCode,
                IsOpen = true,
                OpenAccountDate = DateTime.Now,
                CloseAccountDate = DateTime.MinValue
            };
            _bankAccountDbModels.Add(entity);
        }

        public BankAccount GetAccount(int accountId)
        {
            var entity = _bankAccountDbModels.FirstOrDefault(it => it.Id == accountId);
            if (entity == null)
            {
                throw new ValidationException("This accountId is not found in base!");
            }
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

        public void UpdateBankAccount(BankAccount bankAccount)
        {
            var entity = _bankAccountDbModels.FirstOrDefault(it => it.Id == bankAccount.Id);
            if (entity == null)
            {
                throw new ValidationException("You can't update this bank account, because this id is not found in base!");
            }
            entity.Id = bankAccount.Id;
            entity.UserId = bankAccount.UserId;
            entity.Balance = bankAccount.Balance;
            entity.Currency = bankAccount.Currency;
            entity.IsOpen = bankAccount.IsOpen;
            entity.OpenAccountDate = bankAccount.OpenAccountDate;
            entity.CloseAccountDate = bankAccount.CloseAccountDate;
        }

        public bool HasBankAccounts(int userId)
        {
            return _bankAccountDbModels.FirstOrDefault(it => it.UserId == userId) != null;
        }
    }
}