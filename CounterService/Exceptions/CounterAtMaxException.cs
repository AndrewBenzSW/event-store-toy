using System;
using System.Runtime.Serialization;

namespace CounterService.Exceptions
{
    [Serializable]
    public class CounterAtMaxException : Exception
    {
        public CounterAtMaxException(int maxCount) : this($"Cannot count higher than {maxCount}")
        {
        }

        public CounterAtMaxException(string message) : base(message)
        {
        }

        public CounterAtMaxException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected CounterAtMaxException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}