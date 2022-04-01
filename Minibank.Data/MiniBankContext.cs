using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Minibank.Core.Domains.BankAccounts;
using Minibank.Data.BankAccounts;
using Minibank.Data.MoneyTransferHistory;
using Minibank.Data.Users;

namespace Minibank.Data
{
    public class MiniBankContext : DbContext
    {

        public DbSet<UserDBModel> Users { get; set; }
        
        public DbSet<BankAccountDBModel> BankAccounts { get; set; }
        
        public DbSet<MoneyTransferHistoryDBModel> MoneyTransferHistories { get; set; } 
        

        public MiniBankContext(DbContextOptions options) : base(options)
        {
            
        }

        public class Factory : IDesignTimeDbContextFactory<MiniBankContext>
        {
            public MiniBankContext CreateDbContext(string[] args)
            {
                var options = new DbContextOptionsBuilder().
                    UseNpgsql("Host=localhost;Port=5432;Database=minibank-demo;Username=postgres;Password=123456").Options;
                return new MiniBankContext(options);
            }
        }

    }
}