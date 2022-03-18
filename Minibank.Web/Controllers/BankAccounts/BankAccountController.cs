using Microsoft.AspNetCore.Mvc;
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
        public void Create(string userId, string currencyCode, double startBalance)
        {
            _bankAccountService.CreateBankAccount(userId, currencyCode, startBalance);
        }

        [HttpPatch("/Id/Close")]
        public void CloseAccount(string id)
        {
            _bankAccountService.CloseAccount(id);
        }

        [HttpGet("/Commision")]
        public double GetCommision(double value, string fromAccountId, string toAccountId)
        {
            return _bankAccountService.GetCommision(value, fromAccountId, toAccountId);
        }
        
        [HttpPut("/MoneyTransfer")]
        public void MakeMoneyTransfer(double value, string fromAccountId, string toAccountId)
        {
            _bankAccountService.MakeMoneyTransfer(value, fromAccountId, toAccountId);
        }
    }
}