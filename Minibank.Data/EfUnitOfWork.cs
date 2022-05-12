using System.Threading.Tasks;
using Minibank.Core;

namespace Minibank.Data
{
    public class EfUnitOfWork : IUnitOfWork
    {
        private readonly MiniBankContext _context;

        public EfUnitOfWork(MiniBankContext context)
        {
            _context = context;
        }

        public Task<int> SaveChangesAsync()
        {
            return _context.SaveChangesAsync();
        }
    }
}