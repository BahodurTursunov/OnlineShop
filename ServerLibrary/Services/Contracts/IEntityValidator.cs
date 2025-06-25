namespace ServerLibrary.Services.Contracts
{
    public interface IEntityValidator<T> where T : class
    {
        void Validate(T entity);
    }
}
