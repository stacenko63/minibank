using System;

namespace Minibank.Core
{
    public class CurrencyConverter: ICurrencyConverter
    {
        private readonly IDatabase _database;

        public CurrencyConverter(IDatabase database)
        {
            _database = database; 
        }
        
        public double GetValueInOtherCurrency(double amount, string fromCurrency, string toCurrency)
        {
            if (amount < 0)
            {
                throw new ValidationException("The sum must not be a negative number!");
            }
            return amount * _database.Get(fromCurrency) / _database.Get(toCurrency);
            
        }


    }
}