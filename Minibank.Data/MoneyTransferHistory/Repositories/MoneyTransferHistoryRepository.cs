using System;
using System.Collections.Generic;
using Minibank.Core.Domains.BankAccounts.Repositories;
using Minibank.Core.Domains.MoneyTransferHistory.Repositories;

namespace Minibank.Data.MoneyTransferHistory.Repositories
{
    public class MoneyTransferHistoryRepository : IMoneyTransferRepository
    {
        private static List<MoneyTransferHistoryDBModel> _moneyTransferHistoryDBModels = new List<MoneyTransferHistoryDBModel>();

        public void AddHistory(double value, string currencyCode, string fromAccountId, string toAccountId)
        {
            var entity = new MoneyTransferHistoryDBModel
            {
                Id = Guid.NewGuid().ToString(),
                Value = value,
                CurrencyCode = currencyCode,
                FromAccountId = fromAccountId,
                ToAccountId = toAccountId
            };
            _moneyTransferHistoryDBModels.Add(entity);
        }

    }
}