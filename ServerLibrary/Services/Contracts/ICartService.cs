using BaseLibrary.Entities;

namespace ServerLibrary.Services.Contracts
{
    public interface ICartService
    {
        Task<Cart> GetCartByUserIdAsync(int userId, CancellationToken cancellationToken);
        Task<CartItem> AddItemToCartAsync(int userId, int productId, int quantity, CancellationToken cancellationToken);
        Task<CartItem> UpdateCartItemAsync(int userId, int productId, int quantity, CancellationToken cancellationToken);
        Task<bool> RemoveItemFromCartAsync(int userId, int productId, CancellationToken cancellationToken);
        Task<bool> ClearCartAsync(int userId, CancellationToken cancellationToken);
    }
}
