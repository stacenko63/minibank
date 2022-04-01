using System;
using Microsoft.AspNetCore.Mvc;
using Minibank.Core;

namespace Minibank.Web.Controllers
{
    [ApiController]
    [Route("Converter")]
    public class CurrencyConverterController : ControllerBase
    {
        private readonly ICurrencyConverter _converter;

        public CurrencyConverterController (ICurrencyConverter converter)
        {
            _converter = converter; 
        }

         [HttpGet]
         public double Get(double amount, string fromCurrency, string toCurrency)
         {
             return _converter.GetValueInOtherCurrency(amount, fromCurrency,toCurrency);
         }


         


    }
}