using CounterService;
using CounterService.Extensions;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Threading;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using static CounterEvent;

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
        private string newCounterName;
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
                    new CounterViewModel(Guid.NewGuid(), 6, "Foo"),
                    new CounterViewModel(Guid.NewGuid(), 9, null) { NameChanging = true },
                    new CounterViewModel(Guid.NewGuid(), 0, "Bar"),
                });
            }
            else
            {
                counterApi = new CounterApi();

                AddCounter = new RelayCommand(OnAddCounter);
                Update = new RelayCommand(() => UpdateView());
                Counters = new ObservableCollection<CounterViewModel>();

                CounterStore.EventAdded += (s, e) => UpdateView();
                UpdateView();
            }

            Statistics = new StatisticsViewModel();
        }

        public ICommand AddCounter { get; }
        public ICommand Update { get; }

        public ObservableCollection<CounterViewModel> Counters
        {
            get => counters;
            private set => Set(ref counters, value);
        }

        public string NewCounterName
        {
            get => newCounterName;
            set => Set(ref newCounterName, value);
        }

        public StatisticsViewModel Statistics { get; private set; }

        private async Task UpdateView()
        {
            var events = await CounterStore.AllEventsAfter(lastSequenceSeen).ConfigureAwait(true);

            /// var counterState = new Dictionary<Guid, (string, int)>();

            foreach (var counterEvent in events)
            {
                await DispatcherHelper.RunAsync(() => ApplyEvent(counterEvent.Key, counterEvent.Value));
            }

            timer.Change(5000, Timeout.Infinite);
        }

        private void ApplyEvent(long sequence, CounterEvent counterEvent)
        {
            switch (counterEvent.EventCase)
            {
                case EventOneofCase.Added:
                    Counters.Add(new CounterViewModel(counterEvent.Id.ToGuid(), 0, counterEvent.Added.Name));
                    break;
                case EventOneofCase.Removed:
                    RemoveCounter(counterEvent);
                    break;
                case EventOneofCase.Incremented:
                    UpdateCounter(counterEvent, 1);
                    break;
                case EventOneofCase.Decremented:
                    UpdateCounter(counterEvent, -1);
                    break;
                case EventOneofCase.NameChanged:
                    ChangeCounterName(counterEvent);
                    break;
                default:
                    throw new InvalidOperationException("Oh noes!");
            }

            lastSequenceSeen = sequence;
        }

        private void UpdateCounter(CounterEvent counterEvent, int value)
        {
            var counterId = counterEvent.Id.ToGuid();
            var counter = Counters.Where(x => x.Id == counterId).FirstOrDefault();
            if (counter != null)
            {
                counter.Count += value;
            }
        }

        private void ChangeCounterName(CounterEvent counterEvent)
        {
            var counterId = counterEvent.Id.ToGuid();
            var counter = Counters.Where(x => x.Id == counterId).FirstOrDefault();
            if (counter != null)
            {
                counter.Name = counterEvent.NameChanged.Name;
            }
        }

        private void RemoveCounter(CounterEvent counterEvent)
        {
            var counterId = counterEvent.Id.ToGuid();
            var counter = Counters.Where(x => x.Id == counterId).FirstOrDefault();
            if (counter != null)
            {
                Counters.Remove(counter);
            }
        }

        private Timer timer;

        private async void OnAddCounter()
        {
            var name = string.IsNullOrWhiteSpace(NewCounterName) ? Path.GetRandomFileName() : NewCounterName;
            var id = await counterApi.AddCounter(name).ConfigureAwait(true);
        }
    }
}