using Minibank.Core.Domains.MoneyTransferHistory.Repositories;

namespace Minibank.Core.Domains.MoneyTransferHistory.Services
{
    public class MoneyTransferHistoryService : IMoneyTransferHistoryService
    {
        private readonly IMoneyTransferRepository _moneyTransferRepository;

        public MoneyTransferHistoryService(IMoneyTransferRepository moneyTransferRepository)
        {
            _moneyTransferRepository = moneyTransferRepository;
        }

        public void AddHistory(double value, string currencyCode, string fromAccountId, string toAccountId)
        {
            if (value <= 0) throw new ValidationException("value must be more, than 0!");
            if (currencyCode == null || (currencyCode.ToUpper() != "RUB" && currencyCode.ToUpper() != "USD" &&
                                         currencyCode.ToUpper() != "EUR"))
                throw new ValidationException("This currency is unavailable at the moment!");
            
            _moneyTransferRepository.AddHistory(value, currencyCode, fromAccountId, toAccountId);
        }
    }
}