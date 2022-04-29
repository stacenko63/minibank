using System.Threading.Tasks;

namespace Minibank.Core
{
    public interface ICurrencyConverter
    {
        public Task<double> GetValueInOtherCurrency(double amount, string fromCurrency, string toCurrency);
    }
}