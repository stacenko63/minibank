using System;
using Minibank.Core.Domains.BankAccounts.Repositories;
using Minibank.Core.Domains.MoneyTransferHistory.Services;
using Minibank.Core.Domains.Users.Repositories;
using Minibank.Core.Enums;

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

        public void CreateBankAccount(int userId, string currencyCode, double startBalance)
        {
            if (startBalance < 0)
            {
                throw new ValidationException("StartBalance must be more than 0 or equal 0!");
            }

            if (_userRepository.GetUser(userId) == null)
            {
                throw new ValidationException("UsedId that you want to use to create a bank account is not found!");
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
            _bankAccountRepository.CreateBankAccount(userId, correctCurrencyCode, startBalance);
        }

        public void CloseAccount(int id)
        {
            _bankAccountRepository.CloseAccount(id);
        }

        private double GetCommisionValue(double value, int userId1, int userId2)
        {
            return userId1 == userId2 ? 0 : Math.Floor(Math.Round(value * 0.02, 2));
        }
        
        public double GetCommision(double value, int fromAccountId, int toAccountId)
        {
            if (value <= 0)
            {
                throw new ValidationException("value must be more, than 0!");
            }
            var bankAccount1 = _bankAccountRepository.GetAccount(fromAccountId);
            var bankAccount2 = _bankAccountRepository.GetAccount(toAccountId);
            if (!bankAccount1.IsOpen || !bankAccount2.IsOpen)
            {
                throw new ValidationException("You can't get commision, because one of this accounts is closed!");
            }
            return GetCommisionValue(value, bankAccount1.UserId, bankAccount2.UserId);
        }
        
        public void MakeMoneyTransfer(double value, int fromAccountId, int toAccountId)
        {
            if (value <= 0)
            {
                throw new ValidationException("value must be more, than 0!");
            }
            var bankAccount1 = _bankAccountRepository.GetAccount(fromAccountId);
            var bankAccount2 = _bankAccountRepository.GetAccount(toAccountId);
            if (bankAccount1.Balance < value)
            {
                throw new ValidationException("You don't have enough funds on your balance!");
            }
            if (!bankAccount1.IsOpen || !bankAccount2.IsOpen)
            {
                throw new ValidationException("You can't get commision, because one of this accounts is closed!");
            }
            double commision = _converter.GetValueInOtherCurrency(GetCommisionValue(value, fromAccountId, toAccountId), 
                bankAccount1.Currency, bankAccount2.Currency);
            var valueTo =  _converter.GetValueInOtherCurrency(value, bankAccount1.Currency, bankAccount2.Currency) - commision;
            _moneyTransferHistory.AddHistory(value, bankAccount1.Currency, bankAccount1.Id, bankAccount2.Id);
            _bankAccountRepository.MakeMoneyTransfer(value, valueTo, fromAccountId, toAccountId);
        }
    }
}