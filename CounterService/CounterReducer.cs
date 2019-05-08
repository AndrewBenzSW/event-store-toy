using CounterService.Exceptions;

namespace CounterService
{
    internal class CounterReducer
    {
        internal static Counter Apply(Counter counter, CounterEvent counterEvent)
        {
            switch (counterEvent.EventType)
            {
                case CounterEvents.CounterAdded:
                    return new Counter(counterEvent.CounterId, counterEvent.Version, 0);
                case CounterEvents.CounterRemoved:
                    throw new CounterDeletedException();
                case CounterEvents.CounterIncremented:
                    return new Counter(counterEvent.CounterId, counterEvent.Version, counter.Count + 1);
                case CounterEvents.CounterDecremented:
                    return new Counter(counterEvent.CounterId, counterEvent.Version, counter.Count - 1);
                default:
                    return counter;
            }
        }
    }
}