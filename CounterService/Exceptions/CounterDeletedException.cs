using System;
using System.Runtime.Serialization;

namespace CounterService.Exceptions
{
    [Serializable]
    public class CounterDeletedException : Exception
    {
        public CounterDeletedException()
        {
        }

        public CounterDeletedException(string message) : base(message)
        {
        }

        public CounterDeletedException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected CounterDeletedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}