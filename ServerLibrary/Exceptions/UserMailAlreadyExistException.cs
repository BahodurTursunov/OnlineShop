namespace ServerLibrary.Exceptions
{
    public class UserMailAlreadyExistException : Exception
    {
        public UserMailAlreadyExistException(string message = "Пользователь с такой почтой уже существует") : base(message)
        {

        }
    }
}