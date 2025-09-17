namespace ServerLibrary.Services.Contracts
{
    public interface IBaseService<T> where T : class
    {
        string FirstName { get; set; }
        IQueryable<T> GetAll(CancellationToken cancellationToken);
        Task<T> GetById(int id, CancellationToken cancellationToken);
        Task<T> Create(T entity, CancellationToken cancellationToken);
        Task<T> Update(int id, T entity, CancellationToken cancellationToken);
        Task<T> Delete(int id, CancellationToken cancellationToken);
    }
}
