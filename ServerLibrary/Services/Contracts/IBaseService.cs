namespace ServerLibrary.Services.Contracts
{
    public interface IBaseService<T> where T : class
    {
        IQueryable<T> GetAll();
        Task<T> GetById(int id);
        Task<T> Create(T entity);
        Task<T> Update(int id, T entity);
        Task<T> Delete(int id);
    }
}
