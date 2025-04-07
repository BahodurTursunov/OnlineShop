using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerLibrary.Exceptions
{
    public class UserMailAlreadyExistException : Exception
    {
        public UserMailAlreadyExistException(string message) : base(message)
        {
            
        }
    }
}
