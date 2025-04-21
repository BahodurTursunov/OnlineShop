namespace ServerLibrary.Exceptions
{
    public class DbUpdateDeleteException : Exception
    {
        public DbUpdateDeleteException(string message = "Невозможно удалить или изменить объект, потому что он используется") : base(message) { }

    }
}
