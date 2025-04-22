namespace ServerLibrary.Exceptions
{
    public class UserMailAlreadyExistsException : Exception
    {
        public UserMailAlreadyExistsException(string message = "A user with this email already exists.") : base(message)
        {

        }
    }
}