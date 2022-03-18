using System;
using Minibank.Core.Domains.BankAccounts.Repositories;
using Minibank.Core.Domains.MoneyTransferHistory.Services;
using Minibank.Core.Domains.Users.Repositories;

namespace Minibank.Core.Domains.BankAccounts.Services
{
    public class BankAccountService : IBankAccountService
    {
        private readonly IBankAccountRepository _bankAccountRepository;
        private readonly IUserRepository _userRepository;
        
        private readonly IMoneyTransferHistoryService _moneyTransferHistory;

        private readonly ICurrencyConverter _converter;
        public BankAccountService(IBankAccountRepository bankAccountRepository, IUserRepository userRepository, 
            ICurrencyConverter converter, IMoneyTransferHistoryService moneyTransferHistory)
        {
            _bankAccountRepository = bankAccountRepository;
            _userRepository = userRepository;
            _converter = converter;
            _moneyTransferHistory = moneyTransferHistory;
        }

        public void CreateBankAccount(string userId, string currencyCode, double startBalance)
        {
            if (startBalance < 0) throw new ValidationException("StartBalance must be more than 0 or equal 0!");
            if (currencyCode == null || (currencyCode.ToUpper() != "RUB" && currencyCode.ToUpper() != "USD" &&
                                     currencyCode.ToUpper() != "EUR"))
                throw new ValidationException("This currency is unavailable at the moment!");
            _bankAccountRepository.CreateBankAccount(userId, currencyCode, startBalance);
        }

        public void CloseAccount(string id)
        {
            _bankAccountRepository.CloseAccount(id);
        }

        public double GetCommision(double value, string fromAccountId, string toAccountId)
        {
            if (value <= 0) throw new ValidationException("value must be more, than 0!");
            var bankAccount1 = _bankAccountRepository.GetAccount(fromAccountId);
            var bankAccount2 = _bankAccountRepository.GetAccount(toAccountId);
            if (!bankAccount1.IsOpen || !bankAccount2.IsOpen) 
                throw new ValidationException("You can't get commision, because one of this accounts is closed!");
            return bankAccount1.UserId == bankAccount2.UserId ? 0 : Math.Floor(Math.Round(value * 0.02, 2));
        }
        
        public void MakeMoneyTransfer(double value, string fromAccountId, string toAccountId)
        {
            if (value <= 0) throw new ValidationException("value must be more, than 0!");
            var bankAccount1 = _bankAccountRepository.GetAccount(fromAccountId);
            var bankAccount2 = _bankAccountRepository.GetAccount(toAccountId);
            if (!bankAccount1.IsOpen || !bankAccount2.IsOpen) 
                throw new ValidationException("You can't get commision, because one of this accounts is closed!");
            double commision = _converter.GetValueInOtherCurrency(GetCommision(value, fromAccountId, toAccountId), 
                bankAccount1.Currency, bankAccount2.Currency);
            if (bankAccount1.Balance < value)
                throw new ValidationException("You don't have enough funds on your balance!");
            var valueTo =  _converter.GetValueInOtherCurrency(value, bankAccount1.Currency, bankAccount2.Currency) - commision;
            _moneyTransferHistory.AddHistory(value, bankAccount1.Currency, bankAccount1.Id, bankAccount2.Id);
            _bankAccountRepository.MakeMoneyTransfer(value, valueTo, fromAccountId, toAccountId);
        }
    }
}