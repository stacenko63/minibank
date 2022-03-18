namespace Minibank.Core
{
    public interface ICurrencyConverter
    {
        public double GetValueInOtherCurrency(int value, string currencyCode);
    }
}