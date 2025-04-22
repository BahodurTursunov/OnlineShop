using BaseLibrary.Entities;
using ServerLibrary.Repositories.Contracts;
using ServerLibrary.Services.Contracts;

namespace ServerLibrary.Services
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _repository;

        public CartService(ICartRepository repository)
        {
            _repository = repository;
        }

        public async Task<Cart> GetCartByUserIdAsync(int userId, CancellationToken cancellationToken)
        {
            return await _repository.GetByUserIdAsync(userId, includeItems: true, cancellationToken)
                   ?? throw new InvalidOperationException("Cart not found");
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
            return item;
        }

        public async Task<bool> RemoveItemFromCartAsync(int userId, int productId, CancellationToken cancellationToken)
        {
            var cart = await _repository.GetByUserIdAsync(userId, includeItems: true, cancellationToken);
            if (cart == null) return false;

            var item = cart.CartItems.FirstOrDefault(i => i.ProductId == productId);
            if (item == null) return false;

            cart.CartItems.Remove(item);
            await _repository.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> ClearCartAsync(int userId, CancellationToken cancellationToken)
        {
            var cart = await _repository.GetByUserIdAsync(userId, includeItems: true, cancellationToken);
            if (cart == null || cart.CartItems.Count == 0) return false;

            cart.CartItems.Clear();
            await _repository.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
