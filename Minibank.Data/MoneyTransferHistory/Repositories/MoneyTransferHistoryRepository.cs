using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Minibank.Core.Domains.BankAccounts.Repositories;
using Minibank.Core.Domains.MoneyTransferHistory.Repositories;

namespace Minibank.Data.MoneyTransferHistory.Repositories
{
    public class MoneyTransferHistoryRepository : IMoneyTransferRepository
    {
        private readonly MiniBankContext _context;

        public MoneyTransferHistoryRepository(MiniBankContext context)
        {
            _context = context;
        }

        public async Task AddHistory(double value, string currencyCode, int fromAccountId, int toAccountId)
        {
            await _context.MoneyTransferHistories.AddAsync(new MoneyTransferHistoryDBModel
            {
                Value = value,
                CurrencyCode = currencyCode,
                FromAccountId = fromAccountId,
                ToAccountId = toAccountId
            });
        }

    }
}