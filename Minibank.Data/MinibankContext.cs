using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Minibank.Data.Users;

namespace Minibank.Data
{
    public class MinibankContext : DbContext
    {
        public DbSet<UserDBModel> Users { get; set; }

        public MinibankContext(DbContextOptions options) : base(options)
        {
            
        }

        public class Factory : IDesignTimeDbContextFactory<MinibankContext>
        {
            public MinibankContext CreateDbContext(string[] args)
            {
                var options = new DbContextOptionsBuilder().
                    UseNpgsql("Host=localhost;Port=5432;Database=minibank-demo;Username=postgres;Password=123456").Options;
                return new MinibankContext(options);
            }
        }

    }
}