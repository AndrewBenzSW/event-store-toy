using CounterService.Exceptions;
using CounterService.Extensions;
using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace CounterService
{
    public static class CounterStore
    {
        private static readonly Dictionary<string, Func<byte[], IMessage>> deserializers = new Dictionary<string, Func<byte[], IMessage>>
        {
            { CounterEvents.CounterAdded, CounterAdded.Parser.ParseFrom },
            { CounterEvents.CounterNameChanged, CounterNameChanged.Parser.ParseFrom }
        };

        public static event EventHandler EventAdded;

        /// <summary>
        /// Get a counter entity by id
        /// </summary>
        internal static async Task<Counter> GetById(Guid counterId)
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
                            var payloadBytes = (byte[])reader["Payload"];

                            var counterEvent = new CounterEvent
                            {
                                Id = counterId.ToByteString(),
                                Version = (ulong)version,
                            }.SetEventPayload(type, payloadBytes);

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
        internal static async Task StoreEvent(CounterEvent counterEvent)
        {
            using (var conn = new SqlConnection("Server=localhost;Database=EventStore;Trusted_Connection=True;"))
            {
                using (var comm = conn.CreateCommand())
                {
                    comm.CommandText = "INSERT INTO EventLog (EntityType, EntityId, Version, EventType, Payload) VALUES ('Counter', @EntityId, @Version, @EventType, @Payload)";
                    comm.Parameters.AddWithValue("@EntityID", counterEvent.Id.ToGuid());
                    comm.Parameters.AddWithValue("@Version", Convert.ToInt64(counterEvent.Version));
                    comm.Parameters.AddWithValue("@EventType", counterEvent.EventCase.ToString());
                    comm.Parameters.AddWithValue("@Payload", counterEvent.GetEventPayload().ToByteArray());

                    await conn.OpenAsync().ConfigureAwait(false);

                    try
                    {
                        await comm.ExecuteNonQueryAsync().ConfigureAwait(false);

                        EventAdded?.Invoke(new object(), EventArgs.Empty);
                    }
                    catch (SqlException ex) when (ex.Number == 2627)
                    {
                        throw new CounterOutOfDateException();
                    }
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
                            var payloadBytes = (byte[])reader["Payload"];
                            var sequence = reader.GetInt64(4);

                            events.Add(sequence, new CounterEvent
                            {
                                Id = counterId.ToByteString(),
                                Version = (ulong)version,
                            }.SetEventPayload(type, payloadBytes));
                        }

                        return events;
                    }
                }
            }
        }
    }
}
