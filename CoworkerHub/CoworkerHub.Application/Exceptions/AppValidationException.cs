using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

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
