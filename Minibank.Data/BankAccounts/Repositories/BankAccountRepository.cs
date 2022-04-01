using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Minibank.Core;
using Minibank.Core.Domains.BankAccounts;
using Minibank.Core.Domains.BankAccounts.Repositories;
using Minibank.Core.Domains.MoneyTransferHistory.Services;
using Minibank.Core.Domains.Users.Repositories;

namespace Minibank.Data.BankAccounts.Repositories
{
    public class BankAccountRepository : IBankAccountRepository
    {
        private readonly MiniBankContext _context;
        
        private readonly IUserRepository _userRepository;

        private readonly IMoneyTransferHistoryService _moneyTransferHistory;
        public BankAccountRepository(IUserRepository userRepository, ICurrencyConverter converter, IMoneyTransferHistoryService moneyTransferHistory, MiniBankContext context)
        {
            _userRepository = userRepository;
            _moneyTransferHistory = moneyTransferHistory;
            _context = context;
        }

        public async Task CreateBankAccount(int userId, string currencyCode, double startBalance)
        {
            await _context.BankAccounts.AddAsync(new BankAccountDBModel
            {
                UserId = userId,
                Balance = startBalance,
                Currency = currencyCode,
                IsOpen = true,
                OpenAccountDate = DateTime.Now,
                CloseAccountDate = DateTime.MinValue
            });
        }

        public async Task<BankAccount> GetAccount(int accountId)
        {
            var entity = await _context.BankAccounts.AsNoTracking().FirstOrDefaultAsync(it => it.Id == accountId); 
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

        public async Task UpdateBankAccount(BankAccount bankAccount)
        {
            var entity = await _context.BankAccounts.FirstOrDefaultAsync(it => it.Id == bankAccount.Id);
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

        public async Task<bool> HasBankAccounts(int userId)
        {
            return await _context.BankAccounts.FirstOrDefaultAsync(it => it.UserId == userId) != null;
        }
    }
}