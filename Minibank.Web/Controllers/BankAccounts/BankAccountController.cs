using System.Threading.Tasks;
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
        public async Task Create(int userId, string currencyCode, double startBalance)
        {
            await _bankAccountService.CreateBankAccount(userId, currencyCode, startBalance);
        }

        [HttpPatch("/Id/Close")]
        public async Task CloseAccount(int id)
        {
            await _bankAccountService.CloseAccount(id);
        }

        [HttpGet("/Commision")]
        public async Task<double> GetCommision(double value, int fromAccountId, int toAccountId)
        {
            return await _bankAccountService.GetCommission(value, fromAccountId, toAccountId);
        }
        
        [HttpPost("/MoneyTransfer")]
        public async Task MakeMoneyTransfer(double value, int fromAccountId, int toAccountId)
        {
            await _bankAccountService.MakeMoneyTransfer(value, fromAccountId, toAccountId); 
        }
    }
}