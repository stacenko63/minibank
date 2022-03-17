using System;
using Minibank.Core.Domains.BankAccounts.Repositories;
using Minibank.Core.Domains.Users.Repositories;

namespace Minibank.Core.Domains.BankAccounts.Services
{
    public class BankAccountService : IBankAccountService
    {
        private readonly IBankAccountRepository _bankAccountRepository;
        private readonly IUserRepository _userRepository;

        public BankAccountService(IBankAccountRepository bankAccountRepository, IUserRepository userRepository)
        {
            _bankAccountRepository = bankAccountRepository;
            _userRepository = userRepository;
        }

        public void CreateBankAccount(string userId, string currencyCode, double startBalance)
        {
            if (startBalance < 0) throw new ValidationException("StartBalance must be more than 0 or equal 0!");
            if (currencyCode == null || (currencyCode.ToUpper() != "RUB" && currencyCode.ToUpper() != "USD" &&
                                     currencyCode.ToUpper() != "EUR"))
                throw new ValidationException("This currency is unavailable at the moment!");

            var user = _userRepository.GetUser(userId);
            user.HasBankAccounts = true;
            _userRepository.UpdateUser(user);
            _bankAccountRepository.CreateBankAccount(userId, currencyCode, startBalance);
        }

        public void CloseAccount(string id)
        {
            _bankAccountRepository.CloseAccount(id);
        }

        public double GetCommision(double value, string fromAccountId, string toAccountId)
        {
            if (value <= 0) throw new ValidationException("value must be more, than 0!");
            if (!_bankAccountRepository.GetAccount(fromAccountId).IsOpen ||
                !_bankAccountRepository.GetAccount(toAccountId).IsOpen)
                throw new ValidationException("You can't get commision, because one of this accounts is closed!");
            return _bankAccountRepository.GetCommision(value, fromAccountId, toAccountId);
        }
        
        public void MakeMoneyTransfer(double value, string fromAccountId, string toAccountId)
        {
            if (value <= 0) throw new ValidationException("value must be more, than 0!");
            if (!_bankAccountRepository.GetAccount(fromAccountId).IsOpen ||
                !_bankAccountRepository.GetAccount(toAccountId).IsOpen)
                throw new ValidationException("You can't get money transfer, because one of this accounts is closed!");
            _bankAccountRepository.MakeMoneyTransfer(value, fromAccountId, toAccountId);
        }
        
        
    }
}