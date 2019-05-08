using System;

namespace EventStoreToy.CounterService
{
    public class Counter
    {
        private Guid counterId;
        private readonly long version;

        public Counter()
        {
        }

        public Counter(Guid counterId, long version, int count)
        {
            this.counterId = counterId;
            this.version = version;
            Count = count;
        }

        public int Count { get; }

        internal CounterEvent Remove() =>
            new CounterEvent(counterId, CounterEvents.CounterRemoved, version + 1, "");

        internal CounterEvent Increment()
        {
            if (Count + 1 > 100)
            {
                throw new InvalidOperationException("Cannot count higher than 100");
            }

            return new CounterEvent(counterId, CounterEvents.CounterIncremented, version + 1, "");
        }

        internal CounterEvent Decrement()
        {
            if (Count - 1 < 0)
            {
                throw new InvalidOperationException("Cannot count less than 0");
            }

            return new CounterEvent(counterId, CounterEvents.CounterDecremented, version + 1, "");
        }
    }
}