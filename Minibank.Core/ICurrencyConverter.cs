namespace Minibank.Core
{
    public interface ICurrencyConverter
    {
        public double CurrencyConvert(int value, string currencyCode);
    }
}