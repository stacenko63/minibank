using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Minibank.Core;
using Minibank.Core.Domains.BankAccounts.Repositories;
using Minibank.Core.Domains.MoneyTransferHistory.Repositories;
using Minibank.Core.Domains.Users.Repositories;
using Minibank.Core.Domains.Users.Services;
using Minibank.Data.BankAccounts.Repositories;
using Minibank.Data.MoneyTransferHistory.Repositories;
using Minibank.Data.Users.Repositories;

namespace Minibank.Data
{
    public static class Bootstraps
    {
        public static IServiceCollection AddData(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpClient<IDatabase, Database>(options =>
            {
                options.BaseAddress = new Uri(configuration["CourseBaseUri"]);
            });
            
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IBankAccountRepository, BankAccountRepository>();
            services.AddScoped<IMoneyTransferRepository, MoneyTransferHistoryRepository>();
            
            services.AddScoped<IUnitOfWork, EfUnitOfWork>();
            services.AddDbContext<MiniBankContext>(options => 
                options.UseNpgsql(configuration["DatabaseUrl"]));
            return services; 
        }
    }
}