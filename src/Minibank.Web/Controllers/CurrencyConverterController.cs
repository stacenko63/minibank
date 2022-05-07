using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Minibank.Core;

namespace Minibank.Web.Controllers
{
    [ApiController]
    [Authorize]
    [Route("Converter")]
    public class CurrencyConverterController : ControllerBase
    {
        private readonly ICurrencyConverter _converter;

        public CurrencyConverterController (ICurrencyConverter converter)
        {
            _converter = converter; 
        }

         [HttpGet]
         public async Task<double> Get(double amount, string fromCurrency, string toCurrency)
         {
             return await _converter.GetValueInOtherCurrency(amount, fromCurrency,toCurrency);
         }


         


    }
}