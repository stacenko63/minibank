using System;
using System.Threading.Tasks;

namespace Minibank.Core
{
    public class CurrencyConverter: ICurrencyConverter
    {
        private readonly IDatabase _database;

        public CurrencyConverter(IDatabase database)
        {
            _database = database; 
        }
        
        public async Task<double> GetValueInOtherCurrency(double amount, string fromCurrency, string toCurrency)
        {
            if (amount < 0)
            {
                throw new ValidationException("The sum must not be a negative number!");
            }
            var fromCurrencyValue = await _database.Get(fromCurrency); 
            var toCurrencyValue = await _database.Get(toCurrency);
            return amount * fromCurrencyValue / toCurrencyValue;
        }


    }
}