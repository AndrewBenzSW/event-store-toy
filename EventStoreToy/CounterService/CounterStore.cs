using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace EventStoreToy.CounterService
{
    public static class CounterStore
    {
        public static event EventHandler EventAdded;

        /// <summary>
        /// Get a counter entity by id
        /// </summary>
        public static async Task<Counter> GetById(Guid counterId)
        {
            using (var conn = new SqlConnection("Server=localhost;Database=EventStore;Trusted_Connection=True;"))
            {
                using (var comm = conn.CreateCommand())
                {
                    comm.CommandText = "SELECT Version, EventType, Payload FROM EventLog WHERE EntityType = 'Counter' AND EntityId = @EntityID ORDER BY Version";
                    comm.Parameters.AddWithValue("@EntityID", counterId);

                    await conn.OpenAsync().ConfigureAwait(false);

                    Counter counter = null;

                    using (var reader = await comm.ExecuteReaderAsync().ConfigureAwait(false))
                    {
                        while (await reader.ReadAsync().ConfigureAwait(false))
                        {
                            var version = reader.GetInt64(0);
                            var type = reader.GetString(1);
                            var payload = reader.GetString(2);

                            var counterEvent = new CounterEvent(counterId, type, version, payload);

                            counter = CounterReducer.Apply(counter, counterEvent);
                        }

                        return counter;
                    }
                }
            }
        }

        /// <summary>
        /// Store the given event
        /// </summary>
        public static async Task StoreEvent(CounterEvent counterEvent)
        {
            using (var conn = new SqlConnection("Server=localhost;Database=EventStore;Trusted_Connection=True;"))
            {
                using (var comm = conn.CreateCommand())
                {
                    comm.CommandText = "INSERT INTO EventLog (EntityType, EntityId, Version, EventType, Payload) VALUES ('Counter', @EntityId, @Version, @EventType, @Payload)";
                    comm.Parameters.AddWithValue("@EntityID", counterEvent.CounterId);
                    comm.Parameters.AddWithValue("@Version", counterEvent.Version);
                    comm.Parameters.AddWithValue("@EventType", counterEvent.EventType);
                    comm.Parameters.AddWithValue("@Payload", counterEvent.Payload ?? "");

                    await conn.OpenAsync().ConfigureAwait(false);

                    await comm.ExecuteNonQueryAsync().ConfigureAwait(false);

                    EventAdded?.Invoke(new object(), EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Get all events after the given sequence number
        /// </summary>
        public static async Task<IDictionary<long, CounterEvent>> AllEventsAfter(long startSequence)
        {
            using (var conn = new SqlConnection("Server=localhost;Database=EventStore;Trusted_Connection=True;"))
            {
                using (var comm = conn.CreateCommand())
                {
                    comm.CommandText = "SELECT EntityId, Version, EventType, Payload, Sequence FROM EventLog WHERE EntityType = 'Counter' AND Sequence > @Sequence ORDER BY Sequence";
                    comm.Parameters.AddWithValue("@Sequence", startSequence);

                    await conn.OpenAsync().ConfigureAwait(false);

                    using (var reader = await comm.ExecuteReaderAsync().ConfigureAwait(false))
                    {
                        var events = new Dictionary<long, CounterEvent>();

                        while (await reader.ReadAsync().ConfigureAwait(false))
                        {
                            var counterId = reader.GetGuid(0);
                            var version = reader.GetInt64(1);
                            var type = reader.GetString(2);
                            var payload = reader.GetString(3);
                            var sequence = reader.GetInt64(4);

                            events.Add(sequence, new CounterEvent(counterId, type, version, payload));
                        }

                        return events;
                    }
                }
            }
        }
    }
}
