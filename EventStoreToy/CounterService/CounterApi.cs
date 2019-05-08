using System;
using System.Threading.Tasks;

namespace EventStoreToy.CounterService
{
    /// <summary>
    /// This would be the public API of the Counter Service
    /// </summary>
    public class CounterApi
    {
        /// <summary>
        /// Add a counter
        /// </summary>
        public async Task<Guid> AddCounter()
        {
            var id = Guid.NewGuid();

            await CounterStore.StoreEvent(new CounterEvent(id, CounterEvents.CounterAdded, 0, null)).ConfigureAwait(false);

            return id;
        }

        /// <summary>
        /// Remove a counter
        /// </summary>
        public async Task RemoveCounter(Guid id)
        {
            var counter = await CounterStore.GetById(id).ConfigureAwait(false);

            if (counter != null)
            {
                await CounterStore.StoreEvent(counter.Remove()).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Increment a counter
        /// </summary>
        public async Task Increment(Guid id)
        {
            var counter = await CounterStore.GetById(id).ConfigureAwait(false);

            await CounterStore.StoreEvent(counter.Increment()).ConfigureAwait(false);
        }

        /// <summary>
        /// Decrement a counter
        /// </summary>
        public async Task Decrement(Guid id)
        {
            var counter = await CounterStore.GetById(id).ConfigureAwait(false);

            await CounterStore.StoreEvent(counter.Decrement()).ConfigureAwait(false);
        }
    }
}
