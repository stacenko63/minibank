using Microsoft.AspNetCore.Mvc;
using Minibank.Core.Domains.BankAccounts;
using Minibank.Core.Domains.BankAccounts.Services;
using Minibank.Web.Controllers.BankAccounts.Dto;

namespace Minibank.Web.Controllers.BankAccounts
{
    [ApiController]
    [Route("BankAccount")]
    public class BankAccountController
    {
        
        private readonly IBankAccountService _bankAccountService;
    
        public BankAccountController(IBankAccountService bankAccountService)
        {
            _bankAccountService = bankAccountService;
        }
        
        [HttpPost]
        public void Create(int userId, string currencyCode, double startBalance)
        {
            _bankAccountService.CreateBankAccount(userId, currencyCode, startBalance);
        }

        [HttpPatch("/Id/Close")]
        public void CloseAccount(int id)
        {
            _bankAccountService.CloseAccount(id);
        }

        [HttpGet("/Commision")]
        public double GetCommision(double value, int fromAccountId, int toAccountId)
        {
            return _bankAccountService.GetCommision(value, fromAccountId, toAccountId);
        }
        
        [HttpPost("/MoneyTransfer")]
        public void MakeMoneyTransfer(double value, int fromAccountId, int toAccountId)
        {
            _bankAccountService.MakeMoneyTransfer(value, fromAccountId, toAccountId); 
        }
    }
}