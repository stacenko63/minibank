using System;
using Microsoft.AspNetCore.Mvc;
using Minibank.Core;

namespace Minibank.Web.Controllers
{
    [ApiController]
    [Route("controller")]
    public class CurrencyConverterController : ControllerBase
    {
        private readonly ICurrencyConverter _converter;

        public CurrencyConverterController (ICurrencyConverter converter)
        {
            _converter = converter; 
        }

         [HttpGet]
         public double Get(int value, string currencyCode)
         {
             //if (value < 0) throw new Exception("Сумма в рублях не должна быть меньше 0!");
             var result = _converter.CurrencyConvert(value, currencyCode);
             //if (result == -1) throw new Exception("Указанный вами код валюты отсутствует в нашей базе данных!");
             return result;
         }


         


    }
}