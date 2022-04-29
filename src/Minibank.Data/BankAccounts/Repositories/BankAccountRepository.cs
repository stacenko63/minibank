using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Minibank.Core;
using Minibank.Core.Domains.BankAccounts;
using Minibank.Core.Domains.BankAccounts.Repositories;
using Minibank.Core.Domains.BankAccounts.Services;
using Minibank.Core.Domains.MoneyTransferHistory.Services;
using Minibank.Core.Domains.Users.Repositories;

namespace Minibank.Data.BankAccounts.Repositories
{
    public class BankAccountRepository : IBankAccountRepository
    {
        private readonly MiniBankContext _context;
        public BankAccountRepository(MiniBankContext context)
        {
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
                throw new ValidationException(Messages.NonExistentAccount);
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
            var entity = await _context.BankAccounts.FirstAsync(it => it.Id == bankAccount.Id);
            
            entity.Balance = bankAccount.Balance;
            entity.IsOpen = bankAccount.IsOpen;
            entity.OpenAccountDate = bankAccount.OpenAccountDate;
            entity.CloseAccountDate = bankAccount.CloseAccountDate;
        }

        public async Task<bool> HasBankAccounts(int userId)
        {
            return await _context.BankAccounts.AnyAsync(it => it.UserId == userId);
        }
    }
}