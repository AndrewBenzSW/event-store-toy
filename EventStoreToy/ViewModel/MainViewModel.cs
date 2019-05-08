using EventStoreToy.CounterService;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Threading;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace EventStoreToy.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        private readonly CounterApi counterApi;
        private ObservableCollection<CounterViewModel> counters;
        private long lastSequenceSeen = -1;

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            if (IsInDesignMode)
            {
                // Code runs in Blend --> create design time data.
                Counters = new ObservableCollection<CounterViewModel>(
                    new List<CounterViewModel>
                {
                    new CounterViewModel(Guid.NewGuid(), 6),
                    new CounterViewModel(Guid.NewGuid(), 9),
                    new CounterViewModel(Guid.NewGuid(), 0),
                });
            }
            else
            {
                counterApi = new CounterApi();

                AddCounter = new RelayCommand(OnAddCounter);
                Counters = new ObservableCollection<CounterViewModel>();

                CounterStore.EventAdded += (s, e) => UpdateView();
                UpdateView();
            }

            Statistics = new StatisticsViewModel();
        }

        private async Task UpdateView()
        {
            var events = await CounterStore.AllEventsAfter(lastSequenceSeen).ConfigureAwait(true);

            foreach (var counterEvent in events)
            {
                await DispatcherHelper.RunAsync(() => ApplyEvent(counterEvent.Key, counterEvent.Value));
            }

            timer.Change(5000, Timeout.Infinite);
        }

        private void ApplyEvent(long sequence, CounterEvent counterEvent)
        {
            switch (counterEvent.EventType)
            {
                case CounterEvents.CounterAdded:
                    Counters.Add(new CounterViewModel(counterEvent.CounterId, 0));
                    break;
                case CounterEvents.CounterRemoved:
                    RemoveCounter(counterEvent);
                    break;
                case CounterEvents.CounterIncremented:
                    UpdateCounter(counterEvent, 1);
                    break;
                case CounterEvents.CounterDecremented:
                    UpdateCounter(counterEvent, -1);
                    break;
            }

            lastSequenceSeen = sequence;
        }

        private void UpdateCounter(CounterEvent counterEvent, int value)
        {
            var counter = Counters.Where(x => x.Id == counterEvent.CounterId).FirstOrDefault();
            if (counter != null)
            {
                counter.Count += value;
            }
        }

        private void RemoveCounter(CounterEvent counterEvent)
        {
            var counter = Counters.Where(x => x.Id == counterEvent.CounterId).FirstOrDefault();
            if (counter != null)
            {
                Counters.Remove(counter);
            }
        }

        public ICommand AddCounter { get; }

        private Timer timer;

        public ObservableCollection<CounterViewModel> Counters
        {
            get => counters;
            private set => Set(ref counters, value);
        }

        public StatisticsViewModel Statistics { get; private set; }

        private async void OnAddCounter()
        {
            var id = await counterApi.AddCounter().ConfigureAwait(true);
        }
    }
}