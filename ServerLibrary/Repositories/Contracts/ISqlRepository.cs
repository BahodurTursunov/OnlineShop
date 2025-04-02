using BaseLibrary.Entities;

namespace ServerLibrary.Repositories.Contracts
{
    public interface ISqlRepository<T> where T : BaseEntity
    {
        IQueryable<T> GetAll();
        Task<T> GetById(int id);
        Task<T> CreateAsync(T entity);
        Task<T> UpdateAsync(T entity);
        Task<T> DeleteAsync(int id);
    }
}
