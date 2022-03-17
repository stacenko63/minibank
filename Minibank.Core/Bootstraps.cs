using Microsoft.Extensions.DependencyInjection;
using Minibank.Core.Domains.BankAccounts.Services;
using Minibank.Core.Domains.MoneyTransferHistory.Services;
using Minibank.Core.Domains.Users.Services;

namespace Minibank.Core
{
    public static class Bootstraps
    {
        public static IServiceCollection AddCore(this IServiceCollection services)
        {
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IBankAccountService, BankAccountService>();
            services.AddScoped<IMoneyTransferHistoryService, MoneyTransferHistoryService>();
            services.AddScoped<ICurrencyConverter, CurrencyConverter>();
            return services;
        }
    }
}