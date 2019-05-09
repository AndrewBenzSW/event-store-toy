using CounterService.Exceptions;
using Google.Protobuf;
using System;

namespace CounterService
{
    internal class Counter
    {
        private readonly Guid counterId;
        private readonly string name;
        private readonly ulong version;

        public Counter()
        {
        }

        public Counter(Guid counterId, ulong version, int count, string name)
        {
            this.counterId = counterId;
            this.version = version;
            Count = count;
            this.name = name;
        }

        public int Count { get; }

        internal Counter WithCount(ulong newVersion, int newCount) => new Counter(counterId, newVersion, newCount, name);
        internal Counter WithName(ulong newVersion, string newName) => new Counter(counterId, newVersion, Count, newName);

        internal CounterEvent Remove() =>
            new CounterEvent
            {
                Id = ByteString.CopyFrom(counterId.ToByteArray()),
                Version = version + 1,
                Removed = new CounterRemoved()
            };
        //new CounterEvent(counterId, CounterEvents.CounterRemoved, version + 1, null);

        internal CounterEvent Increment()
        {
            if (Count + 1 > 100)
            {
                throw new CounterAtMaxException(100);
            }

            return new CounterEvent
            {
                Id = ByteString.CopyFrom(counterId.ToByteArray()),
                Version = version + 1,
                Incremented = new CounterIncremented()
            };
            //return new CounterEvent(counterId, CounterEvents.CounterIncremented, version + 1, null);
        }

        internal CounterEvent Decrement()
        {
            if (Count - 1 < 0)
            {
                throw new CounterAtMinException(0);
            }

            return new CounterEvent
            {
                Id = ByteString.CopyFrom(counterId.ToByteArray()),
                Version = version + 1,
                Decremented = new CounterDecremented()
            };
            //return new CounterEvent(counterId, CounterEvents.CounterDecremented, version + 1, null);
        }

        internal CounterEvent ChangeName(string newName, string originalName)
        {
            if (!string.IsNullOrWhiteSpace(name) && name != originalName)
            {
                throw new CounterOutOfDateException();
            }

            return new CounterEvent
            {
                Id = ByteString.CopyFrom(counterId.ToByteArray()),
                Version = version + 1,
                NameChanged = new CounterNameChanged { Name = newName }
            };
            //return new CounterEvent(counterId, CounterEvents.CounterNameChanged, version + 1, payload);
        }
    }
}