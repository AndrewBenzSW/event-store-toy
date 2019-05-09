using CounterService;
using CounterService.Exceptions;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Threading;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CounterRandomizer.ViewModel
{
    public class AutomatedCounter : ViewModelBase
    {
        private CounterApi counterApi = new CounterApi();
        private bool running;
        private long eventsSent;
        private string message;
        private Timer timer;
        private int interval = 500;
        private string name = string.Empty;
        private static Random random = new Random();

        private Func<Task> updateFunc;

        public AutomatedCounter(Guid id, string name)
        {
            Id = id;
            Name = name;

            if (!IsInDesignMode)
            {
                Increment = new RelayCommand(OnIncrement, () => !Running);
                Decrement = new RelayCommand(OnDecrement, () => !Running);
                Random = new RelayCommand(OnRandom, () => !Running);
                Stop = new RelayCommand(OnStop, () => Running);

                timer = new Timer(OnTimerTick, null, Timeout.Infinite, Timeout.Infinite);
            }
        }

        public Guid Id { get; }

        public string Name
        {
            get => name;
            set => Set(ref name, value);
        }

        public long EventsSent
        {
            get => eventsSent;
            set => Set(ref eventsSent, value);
        }

        public string Message
        {
            get => message;
            set => Set(ref message, value);
        }

        public bool Running
        {
            get => running;
            set => Set(ref running, value);
        }

        public int Interval
        {
            get => interval;
            set => Set(ref interval, value);
        }

        public ICommand Increment { get; }
        public ICommand Decrement { get; }
        public ICommand Random { get; }
        public ICommand Stop { get; }

        private void OnStop()
        {
            timer.Change(Timeout.Infinite, Timeout.Infinite);
            Running = false;
        }

        private void OnRandom()
        {
            Running = true;
            updateFunc = async () =>
            {
                var val = random.Next(0, 3);
                if (val == 0)
                {
                    await Perform(counterApi.Increment).ConfigureAwait(false);
                }
                else if (val == 2)
                {
                    await Perform(counterApi.Decrement).ConfigureAwait(false);
                }
            };

            timer.Change(TimeSpan.Zero, Timeout.InfiniteTimeSpan);
        }

        private void OnDecrement()
        {
            Running = true;
            updateFunc = () => Perform(counterApi.Decrement);
            timer.Change(TimeSpan.Zero, Timeout.InfiniteTimeSpan);
        }

        private void OnIncrement()
        {
            Running = true;
            updateFunc = () => Perform(counterApi.Increment);
            timer.Change(TimeSpan.Zero, Timeout.InfiniteTimeSpan);
        }

        private async void OnTimerTick(object state)
        {
            try
            {
                await DispatcherHelper.RunAsync(() => Message = "Sending Events");
                await updateFunc().ConfigureAwait(false);
            }
            catch (CounterDeletedException)
            {
                await DispatcherHelper.RunAsync(() =>
                {
                    OnStop();
                    Message = "Counter deleted";
                });

                return;
            }
            catch (Exception ex)
            {
                await DispatcherHelper.RunAsync(() => Message = ex.Message);
            }

            timer.Change(TimeSpan.FromMilliseconds(Interval), Timeout.InfiniteTimeSpan);
        }

        private async Task Perform(Func<Guid, Task> action)
        {
            await action(Id).ConfigureAwait(false);
            await DispatcherHelper.RunAsync(() => EventsSent += 1);
        }
    }
}