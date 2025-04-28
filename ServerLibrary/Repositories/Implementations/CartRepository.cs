using BaseLibrary.Entities;
using Microsoft.EntityFrameworkCore;
using ServerLibrary.Data;
using ServerLibrary.Repositories.Contracts;

namespace ServerLibrary.Repositories.Implementations
{
    public class CartRepository : ICartRepository
    {
        private readonly ApplicationDbContext _context;

        public CartRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Cart?> GetByUserIdAsync(int userId, bool includeItems = false, CancellationToken cancellationToken = default)
        {
            var query = _context.Carts.AsQueryable();

            if (includeItems)
            {
                query = query.Include(c => c.CartItems)
                             .ThenInclude(i => i.Product);
            }

            return await query.FirstOrDefaultAsync(c => c.UserId == userId, cancellationToken);
        }

        public async Task<Product?> GetProductByIdAsync(int productId, CancellationToken cancellationToken = default)
        {
            return await _context.Products.FirstOrDefaultAsync(p => p.Id == productId, cancellationToken);
        }

        public async Task AddCartAsync(Cart cart, CancellationToken cancellationToken = default)
        {
            await _context.Carts.AddAsync(cart, cancellationToken);
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
