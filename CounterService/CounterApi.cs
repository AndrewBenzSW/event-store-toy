using Google.Protobuf;
using System;
using System.Threading.Tasks;

namespace CounterService
{
    /// <summary>
    /// This would be the public API of the Counter Service
    /// </summary>
    public class CounterApi
    {
        /// <summary>
        /// Add a counter
        /// </summary>
        public async Task<Guid> AddCounter(string name)
        {
            var id = Guid.NewGuid();

            //var payload = new CounterAdded { Name = "" };
            var counterEvent = new CounterEvent
            {
                Id = ByteString.CopyFrom(id.ToByteArray()),
                Version = 0,
                Added = new CounterAdded { Name = "" }
            };
            await CounterStore.StoreEvent(counterEvent).ConfigureAwait(false);

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

        /// <summary>
        /// Change the name of a counter
        /// </summary>
        public async Task ChangeName(Guid id, string newName, string originalName)
        {
            var counter = await CounterStore.GetById(id).ConfigureAwait(false);

            await CounterStore.StoreEvent(counter.ChangeName(newName, originalName)).ConfigureAwait(false);
        }
    }
}
