using System.Threading.Tasks;

namespace Minibank.Core
{
    public interface IDatabase
    {
        public Task<double> GetCurrencyValueInRubles(string currencyCode);
    }
}