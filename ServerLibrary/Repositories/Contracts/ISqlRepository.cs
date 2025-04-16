using BaseLibrary.Entities;

namespace ServerLibrary.Repositories.Contracts
{
    public interface ISqlRepository<T> where T : BaseEntity
    {
        IQueryable<T> GetAll(CancellationToken cancellationToken);
        Task<T> GetById(int id, CancellationToken cancellationToken);
        Task<T> CreateAsync(T entity, CancellationToken cancellationToken);
        Task<T> UpdateAsync(T entity, CancellationToken cancellationToken);
        Task<T> DeleteAsync(int id, CancellationToken cancellationToken);
    }
}
