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
            return (double) value / _database.Get(currencyCode); 
        }


    }
}