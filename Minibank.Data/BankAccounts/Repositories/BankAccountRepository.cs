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
        
        public BankAccountRepository(IUserRepository userRepository, ICurrencyConverter converter, IMoneyTransferHistoryService moneyTransferHistory)
        {
            _userRepository = userRepository;
            _moneyTransferHistory = moneyTransferHistory;
        }

        public void CreateBankAccount(string userId, string currencyCode, double startBalance)
        {
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

        public void CloseAccount(string id)
        {
            var entity = _bankAccountDbModels.FirstOrDefault(it => it.Id == id);
            if (entity == null) throw new ValidationException("This BankAccount Id is not fount in base!");
            if (entity.Balance != 0)
                throw new ValidationException("Before closing BankAccount your balance must be 0!");
            if (!entity.IsOpen) throw new ValidationException("This account has already closed!");
            entity.IsOpen = false;
            entity.CloseAccountDate = DateTime.Now;
        }
        
        public void MakeMoneyTransfer(double valueFrom, double valueTo, string fromAccountId, string toAccountId)
        {
            _bankAccountDbModels.FirstOrDefault(it => it.Id == fromAccountId).Balance -= valueFrom;
            _bankAccountDbModels.FirstOrDefault(it => it.Id == toAccountId).Balance += valueTo;
        }

        public bool HasBankAccounts(string userId)
        {
            return _bankAccountDbModels.FirstOrDefault(it => it.UserId == userId) != null;
        }
    }
}