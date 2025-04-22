using BaseLibrary.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ServerLibrary.Data;
using ServerLibrary.Services.Contracts;

namespace ServerLibrary.Services.Implementations
{
    public class CartService : ICartService
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<CartService> _logger;

        public CartService(ApplicationDbContext db, ILogger<CartService> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<CartItem> AddItemToCartAsync(int userId, int productId, int quantity, CancellationToken cancellationToken)
        {
            if (quantity <= 0)
            {
                throw new ArgumentException("Quantity must be greater than zero.", nameof(quantity));
            }

            var cart = await _db.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == userId, cancellationToken);

            if (cart == null)
            {
                cart = new Cart
                {
                    UserId = userId,
                    Created

                };

            }
        }

        public Task<bool> ClearCartAsync(int userId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<Cart> GetCartByUserIdAsync(int userId, CancellationToken cancellationToken)
        {
            return await _db.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId, cancellationToken);
        }

        public Task<bool> RemoveItemFromCartAsync(int userId, int productId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<CartItem> UpdateCartItemAsync(int userId, int productId, int quantity, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
