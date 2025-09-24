namespace ServerLibrary.Services.Contracts.Validation
{
    public interface IEntityValidator<T> where T : class
    {
        void Validate(T entity);
    }
}
