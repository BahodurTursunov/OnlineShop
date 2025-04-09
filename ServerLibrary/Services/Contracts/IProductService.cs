using BaseLibrary.Entities;

namespace ServerLibrary.Services.Contracts
{
    public interface IProductService : IBaseService<Product>
    {
        Task<Product> GetByName(string name);
    }
}
