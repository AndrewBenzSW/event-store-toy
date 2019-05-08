using System;
using System.Runtime.Serialization;

namespace CounterService.Exceptions
{
    [Serializable]
    public class CounterOutOfDateException : Exception
    {
        public CounterOutOfDateException() : this("Counter is out of date, try again")
        {
        }

        public CounterOutOfDateException(string message) : base(message)
        {
        }

        public CounterOutOfDateException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected CounterOutOfDateException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}