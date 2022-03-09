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
        
        public double GetValueInOtherCurrency(int value, string currencyCode)
        {
            if (value < 0) throw new UserFriendlyException("The sum must not be a negative number!");
            int currencyRate = _database.Get(currencyCode);
            if (currencyRate == -1) throw new UserFriendlyException("The code of the specified currency was not found in our database!"); 
            return (double) value / currencyRate; 
        }


    }
}