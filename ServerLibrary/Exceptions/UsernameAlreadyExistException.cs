namespace ServerLibrary.Exceptions
{
    public class UsernameAlreadyExistsException : Exception
    {
        public UsernameAlreadyExistsException(string message = "A user with this username already exists.") : base(message) { }

    }
}
