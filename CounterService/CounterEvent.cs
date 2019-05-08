using System;

namespace CounterService
{
    public class CounterEvent
    {
        internal CounterEvent(Guid counterId, string type, long version, string payload)
        {
            CounterId = counterId;
            Version = version;
            EventType = type;
            Payload = payload;
        }

        public Guid CounterId { get; }
        public long Version { get; }
        public string EventType { get; }
        public string Payload { get; }
    }
}