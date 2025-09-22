using BaseLibrary.Entities;
using Microsoft.Extensions.Logging;
using ServerLibrary.Repositories.Contracts;
using ServerLibrary.Services.Contracts;
using ServerLibrary.Services.Contracts.Cache;

namespace ServerLibrary.Services.Implementations
{
    public class CartService(ICartRepository repository, IRedisCacheService<Cart> cartCache, ILogger<CartService> logger) : ICartService
    {
        private readonly ICartRepository _repository = repository;
        private readonly IRedisCacheService<Cart> _cartCache = cartCache;
        private readonly ILogger<CartService> _logger = logger;

        public async Task<Cart> GetCartByUserIdAsync(int userId, CancellationToken cancellationToken)
        {
            string cachedKey = $"cart:user:{userId}";

            var cachedCart = await _cartCache.GetAsync(cachedKey, cancellationToken);

            if (cachedCart != null)
            {
                _logger.LogInformation($"Retrieved cart from cache for user {userId}");
                return cachedCart;
            }

            _logger.LogInformation("Cart not found in cache. Fetching from repository for user {UserId}", userId);
            var cartFromDb = await _repository.GetByUserIdAsync(userId, includeItems: true, cancellationToken)
                             ?? throw new InvalidOperationException("Cart not found");

            await _cartCache.SetAsync(cachedKey, cartFromDb, TimeSpan.FromMinutes(30), cancellationToken: cancellationToken);
            _logger.LogInformation("Stored cart in cache for user {UserId}", userId);

            return cartFromDb;

        }

        public async Task<CartItem> AddItemToCartAsync(int userId, int productId, int quantity, CancellationToken cancellationToken)
        {
            var cart = await _repository.GetByUserIdAsync(userId, includeItems: true, cancellationToken);

            var product = await _repository.GetProductByIdAsync(productId, cancellationToken)
                          ?? throw new InvalidOperationException("Product not found");

            if (cart == null)
            {
                cart = new Cart
                {
                    UserId = userId,
                    CartItems = new List<CartItem>()
                };

                var item = new CartItem
                {
                    ProductId = productId,
                    Quantity = quantity,
                    Amount = quantity * product.Price
                };

                cart.CartItems.Add(item);
                await _repository.AddCartAsync(cart, cancellationToken);
            }
            else
            {
                var existing = cart.CartItems.FirstOrDefault(i => i.ProductId == productId);
                if (existing != null)
                {
                    existing.Quantity += quantity;
                    existing.Amount = existing.Quantity * product.Price;
                }
                else
                {
                    cart.CartItems.Add(new CartItem
                    {
                        ProductId = productId,
                        Quantity = quantity,
                        Amount = quantity * product.Price
                    });
                }
            }

            await _repository.SaveChangesAsync(cancellationToken);

            await InvalidateCartCacheAsync(userId, cancellationToken);

            return cart.CartItems.First(i => i.ProductId == productId);
        }

        public async Task<CartItem> UpdateCartItemAsync(int userId, int productId, int quantity, CancellationToken cancellationToken)
        {
            var cart = await _repository.GetByUserIdAsync(userId, includeItems: true, cancellationToken)
                       ?? throw new InvalidOperationException("Cart not found");

            var item = cart.CartItems.FirstOrDefault(i => i.ProductId == productId)
                       ?? throw new InvalidOperationException("Item not found");

            if (quantity <= 0)
            {
                cart.CartItems.Remove(item);
            }
            else
            {
                var product = await _repository.GetProductByIdAsync(productId, cancellationToken)
                              ?? throw new InvalidOperationException("Product not found");

                item.Quantity = quantity;
                item.Amount = quantity * product.Price;
            }

            await _repository.SaveChangesAsync(cancellationToken);

            await InvalidateCartCacheAsync(userId, cancellationToken);

            return item;
        }

        public async Task<bool> RemoveItemFromCartAsync(int userId, int productId, CancellationToken cancellationToken)
        {
            var cart = await _repository.GetByUserIdAsync(userId, includeItems: true, cancellationToken);
            if (cart == null)
            {
                return false;
            }

            var item = cart.CartItems.FirstOrDefault(i => i.ProductId == productId);
            if (item == null)
            {
                return false;
            }

            cart.CartItems.Remove(item);
            await InvalidateCartCacheAsync(userId, cancellationToken);

            await _repository.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> ClearCartAsync(int userId, CancellationToken cancellationToken)
        {
            var cart = await _repository.GetByUserIdAsync(userId, includeItems: true, cancellationToken);
            if (cart == null || cart.CartItems.Count == 0)
            {
                return false;
            }

            cart.CartItems.Clear();
            await _repository.SaveChangesAsync(cancellationToken);
            return true;
        }
        private async Task InvalidateCartCacheAsync(int userId, CancellationToken cancellationToken)
        {
            try
            {
                string cacheKey = $"cart:user:{userId}";
                await _cartCache.RemoveAsync(cacheKey, cancellationToken);
                _logger.LogInformation("Invalidated cart cache for user {UserId}", userId);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to invalidate cart cache for user {UserId}", userId);
            }
        }
    }
}
