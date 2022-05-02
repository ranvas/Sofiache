using Core.Abstractions.Dispatcher;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dispatcher
{
    public class RequestException : Exception, IDispatchedRequest
    {
        public RequestException() { }
        public RequestException(string? message) : base(message) { }
        public RequestException(string? message, Exception? innerException) : base(message, innerException) { }
        protected RequestException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
