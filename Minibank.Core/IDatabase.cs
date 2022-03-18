namespace Minibank.Core
{
    public interface IDatabase
    {
        public int Get(string currencyCode);
    }
}