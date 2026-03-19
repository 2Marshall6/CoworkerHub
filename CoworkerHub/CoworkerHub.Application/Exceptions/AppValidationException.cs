
using System.Runtime.Serialization;

namespace CoworkerHub.Application.Exceptions
{
    public class AppValidationException : Exception
    {
        public AppValidationException()
        {
        }

        public AppValidationException(string? message) : base(message)
        {
        }

        public AppValidationException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected AppValidationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
