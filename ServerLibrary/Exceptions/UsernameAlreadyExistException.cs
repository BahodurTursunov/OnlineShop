namespace ServerLibrary.Exceptions
{
    public class UsernameAlreadyExitstException : Exception
    {
        public UsernameAlreadyExitstException(string message = "Пользователь с таким логином уже существует") : base(message) { }

    }
}
