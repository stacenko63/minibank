using System;
using System.Threading.Tasks;
using Minibank.Core.Domains.MoneyTransferHistory.Repositories;
using Minibank.Core.Enums;

namespace Minibank.Core.Domains.MoneyTransferHistory.Services
{
    public class MoneyTransferHistoryService : IMoneyTransferHistoryService
    {
        private readonly IMoneyTransferRepository _moneyTransferRepository;

        private readonly IUnitOfWork _unitOfWork;

        public MoneyTransferHistoryService(IMoneyTransferRepository moneyTransferRepository, IUnitOfWork unitOfWork)
        {
            _moneyTransferRepository = moneyTransferRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task AddHistory(double value, string currencyCode, int fromAccountId, int toAccountId)
        {
            if (value <= 0)
            {
                throw new ValidationException("value must be more, than 0!");
            }
            
            if (currencyCode == null)
            {
                throw new ValidationException("This currency is unavailable at the moment!");
            }
            
            string correctCurrencyCode = Char.ToString(currencyCode[0]).ToUpper() + 
                                         currencyCode.Substring(1,currencyCode.Length-1).ToLower();
            
            if (!Enum.IsDefined(typeof(PermittedCurrencies), correctCurrencyCode))
            {
                throw new ValidationException("This currency is unavailable at the moment!");
            }
            
            await _moneyTransferRepository.AddHistory(value, correctCurrencyCode, fromAccountId, toAccountId);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}