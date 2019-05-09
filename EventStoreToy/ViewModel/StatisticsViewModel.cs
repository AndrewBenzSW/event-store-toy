using CounterService;
using CounterService.Extensions;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Threading;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using static CounterEvent;

namespace EventStoreToy.ViewModel
{
    public class StatisticsViewModel : ViewModelBase
    {
        private readonly Timer timer;

        private bool working;
        private int sum;
        private double average;
        private double standardDeviation;
        private readonly bool resetRequested;

        public StatisticsViewModel()
        {
            if (IsInDesignMode)
            {
                Sum = 201;
                Average = 15.23;
                StandardDeviation = 0.6;
            }
            else
            {
                //timer = new Timer(OnTimerTick, null, 0, Timeout.Infinite);
                Reset = new RelayCommand(OnReset, () => !Working);
                Update = new RelayCommand(OnUpdate, () => !Working);
            }
        }

        public ICommand Update { get; }
        public ICommand Reset { get; }

        public int Sum
        {
            get => sum;
            set => Set(ref sum, value);
        }

        public double Average
        {
            get => average;
            set => Set(ref average, value);
        }

        public double StandardDeviation
        {
            get => standardDeviation;
            set => Set(ref standardDeviation, value);
        }

        public bool Working
        {
            get => working;
            set => Set(ref working, value);
        }

        protected async void OnUpdate()
        {
            Working = true;

            var lastSequenceSeen = await GetLastEventSeen().ConfigureAwait(false);

            var events = await CounterStore.AllEventsAfter(lastSequenceSeen).ConfigureAwait(true);

            foreach (var counterEvent in events)
            {
                using (var conn = new SqlConnection("Server=localhost;Database=EventStore;Trusted_Connection=True;"))
                {
                    await conn.OpenAsync().ConfigureAwait(false);

                    using (var transaction = conn.BeginTransaction())
                    {
                        switch (counterEvent.Value.EventCase)
                        {
                            case EventOneofCase.Added:
                                await CreateCounter(conn, transaction, counterEvent.Value).ConfigureAwait(false);
                                break;
                            case EventOneofCase.Removed:
                                await RemoveCounter(conn, transaction, counterEvent.Value).ConfigureAwait(false);
                                break;
                            case EventOneofCase.Incremented:
                                await UpdateCounter(conn, transaction, counterEvent.Value, 1).ConfigureAwait(false);
                                break;
                            case EventOneofCase.Decremented:
                                await UpdateCounter(conn, transaction, counterEvent.Value, -1).ConfigureAwait(false);
                                break;
                        }

                        await UpdateLastSequenceSeen(conn, transaction, counterEvent.Key).ConfigureAwait(false);
                        transaction.Commit();
                    }
                }
            }

            var stats = await GetStatistics().ConfigureAwait(false);

            await DispatcherHelper.RunAsync(() =>
            {
                Sum = stats.Sum;
                Average = stats.Average;
                StandardDeviation = stats.StandardDeviation;
                Working = false;
            });

            //timer.Change(5000, Timeout.Infinite);
        }

        private async Task ResetCachedData()
        {
            Working = true;

            using (var conn = new SqlConnection("Server=localhost;Database=EventStore;Trusted_Connection=True;"))
            {
                await conn.OpenAsync().ConfigureAwait(false);

                using (var transaction = conn.BeginTransaction())
                {
                    using (var comm = conn.CreateCommand())
                    {
                        comm.Transaction = transaction;
                        comm.CommandText = "TRUNCATE TABLE CounterStatistics";

                        await comm.ExecuteNonQueryAsync().ConfigureAwait(false);
                    }

                    using (var comm = conn.CreateCommand())
                    {
                        comm.Transaction = transaction;
                        comm.CommandText = "UPDATE CounterStatisticsSequence SET LastEventSeen = -1";

                        await comm.ExecuteNonQueryAsync().ConfigureAwait(false);
                    }

                    transaction.Commit();
                }
            }

            await DispatcherHelper.RunAsync(() =>
            {
                Sum = 0;
                Average = 0;
                StandardDeviation = 0;
                Working = false;
            });
        }

        private async Task<long> GetLastEventSeen()
        {
            using (var conn = new SqlConnection("Server=localhost;Database=EventStore;Trusted_Connection=True;"))
            {
                using (var comm = conn.CreateCommand())
                {
                    comm.CommandText = "SELECT LastEventSeen FROM CounterStatisticsSequence";
                    await conn.OpenAsync().ConfigureAwait(false);

                    var result = await comm.ExecuteScalarAsync().ConfigureAwait(false);
                    return result != null ? (long)result : -1;
                }
            }
        }

        private async Task<Stats> GetStatistics()
        {
            using (var conn = new SqlConnection("Server=localhost;Database=EventStore;Trusted_Connection=True;"))
            {
                using (var comm = conn.CreateCommand())
                {
                    comm.CommandText = "SELECT SUM(Count) as Sum, AVG(CAST(Count AS float)) as Average, STDEV(Count) as StandardDeviation FROM CounterStatistics";
                    await conn.OpenAsync().ConfigureAwait(false);

                    using (var reader = await comm.ExecuteReaderAsync().ConfigureAwait(false))
                    {
                        if (await reader.ReadAsync().ConfigureAwait(false))
                        {
                            var sumValue = reader.GetInt32(0);
                            var averageValue = reader.GetDouble(1);
                            var standardDeviationValue = 0.0;

                            if (!reader.IsDBNull(2))
                            {
                                standardDeviationValue = reader.GetDouble(2);
                            }

                            return new Stats(sumValue, averageValue, standardDeviationValue);
                        }

                        return new Stats();
                    }
                }
            }
        }

        private async Task UpdateLastSequenceSeen(SqlConnection conn, SqlTransaction transaction, long key)
        {
            using (var comm = conn.CreateCommand())
            {
                comm.Transaction = transaction;
                comm.CommandText = "UPDATE CounterStatisticsSequence SET LastEventSeen = @Sequence";
                comm.Parameters.AddWithValue("@Sequence", key);

                await comm.ExecuteNonQueryAsync().ConfigureAwait(false);
            }
        }

        private async Task UpdateCounter(SqlConnection conn, SqlTransaction transaction, CounterEvent counterEvent, int value)
        {
            using (var comm = conn.CreateCommand())
            {
                comm.Transaction = transaction;
                comm.CommandText = "UPDATE CounterStatistics SET Count = Count + @Value WHERE CounterId = @CounterId";
                comm.Parameters.AddWithValue("@CounterId", counterEvent.Id.ToGuid());
                comm.Parameters.AddWithValue("@Value", value);

                await comm.ExecuteNonQueryAsync().ConfigureAwait(false);
            }
        }

        private async Task RemoveCounter(SqlConnection conn, SqlTransaction transaction, CounterEvent counterEvent)
        {
            using (var comm = conn.CreateCommand())
            {
                comm.Transaction = transaction;
                comm.CommandText = "DELETE FROM CounterStatistics WHERE CounterId = @CounterId";
                comm.Parameters.AddWithValue("@CounterId", counterEvent.Id.ToGuid());

                await comm.ExecuteNonQueryAsync().ConfigureAwait(false);
            }
        }

        private async Task CreateCounter(SqlConnection conn, SqlTransaction transaction, CounterEvent counterEvent)
        {
            using (var comm = conn.CreateCommand())
            {
                comm.Transaction = transaction;
                comm.CommandText = "INSERT INTO CounterStatistics (CounterId, Count) VALUES (@CounterId, 0)";
                comm.Parameters.AddWithValue("@CounterId", counterEvent.Id.ToGuid());

                await comm.ExecuteNonQueryAsync().ConfigureAwait(false);
            }
        }

        private async void OnReset()
        {
            await ResetCachedData().ConfigureAwait(false);
        }
    }
}