namespace Minibank.Core
{
    public interface ICurrencyConverter
    {
        public double GetValueInOtherCurrency(double amount, string fromCurrency, string toCurrency);
    }
}