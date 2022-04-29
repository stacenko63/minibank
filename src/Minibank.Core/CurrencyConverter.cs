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
            
            return await _database.GetCurrencyValueInRubles(fromCurrency) / 
                await _database.GetCurrencyValueInRubles(toCurrency) * amount;
        }


    }
}