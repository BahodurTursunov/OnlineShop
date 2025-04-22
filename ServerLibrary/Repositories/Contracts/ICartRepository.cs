using BaseLibrary.Entities;

namespace ServerLibrary.Repositories.Contracts
{
    public interface ICartRepository
    {
        Task<Cart?> GetByUserIdAsync(int userId, bool includeItems = false, CancellationToken cancellationToken = default);
        Task<Product?> GetProductByIdAsync(int productId, CancellationToken cancellationToken = default);
        Task AddCartAsync(Cart cart, CancellationToken cancellationToken = default);
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
