using System;
using System.Runtime.Serialization;

namespace CounterService.Exceptions
{
    [Serializable]
    public class CounterAtMinException : Exception
    {
        public CounterAtMinException(int minCount) : this($"Cannot count lower than {minCount}")
        {
        }

        public CounterAtMinException(string message) : base(message)
        {
        }

        public CounterAtMinException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected CounterAtMinException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}