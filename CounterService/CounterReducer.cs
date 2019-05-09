using CounterService.Exceptions;
using System;

namespace CounterService
{
    internal static class CounterReducer
    {
        internal static Counter Apply(Counter counter, CounterEvent counterEvent)
        {
            switch (counterEvent.EventCase)
            {
                case CounterEvent.EventOneofCase.Added:
                    return new Counter(new Guid(counterEvent.Id.ToByteArray()), counterEvent.Version, 0, counterEvent.Added.Name);
                case CounterEvent.EventOneofCase.Removed:
                    throw new CounterDeletedException();
                case CounterEvent.EventOneofCase.Incremented:
                    return counter.WithCount(counterEvent.Version, counter.Count + 1);
                case CounterEvent.EventOneofCase.Decremented:
                    return counter.WithCount(counterEvent.Version, counter.Count - 1);
                case CounterEvent.EventOneofCase.NameChanged:
                    return counter.WithName(counterEvent.Version, counterEvent.NameChanged.Name);
                default:
                    return counter;
            }
        }
    }
}