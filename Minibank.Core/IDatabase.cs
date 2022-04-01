namespace Minibank.Core
{
    public interface IDatabase
    {
        public double Get(string currencyCode);
    }
}