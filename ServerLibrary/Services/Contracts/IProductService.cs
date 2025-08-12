using BaseLibrary.Entities;

namespace ServerLibrary.Services.Contracts
{
    public interface IProductService : IBaseService<Product>
    {
        Task<Product> GetByName(string name, CancellationToken cancellationToken);
        Task<List<Product>> GetAllCached(CancellationToken cancellationToken = default);
    }
}
