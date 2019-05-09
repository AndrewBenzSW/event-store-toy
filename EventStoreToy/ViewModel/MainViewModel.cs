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
            var maxSequence = lastSequenceSeen;
            var events = await CounterStore.AllEventsAfter(lastSequenceSeen).ConfigureAwait(true);
            var counterState = Counters.ToDictionary(x => x.Id, x => new CounterState(x.Name, x.Count));

            foreach (var counterEvent in events)
            {
                CounterState counterDetails = null;
                var counterId = counterEvent.Value.Id.ToGuid();

                if (!counterState.TryGetValue(counterEvent.Value.Id.ToGuid(), out counterDetails))
                {
                    counterDetails = null;
                }

                counterState[counterId] = ApplyEvent(counterDetails, counterEvent.Value);
                maxSequence = counterEvent.Key;
            }

            await DispatcherHelper.RunAsync(() =>
            {
                var newCounters = counterState
                    .Where(x => x.Value != null);

                foreach (var counter in newCounters)
                {
                    var viewModel = Counters.FirstOrDefault(x => x.Id == counter.Key);
                    if (viewModel == null)
                    {
                        Counters.Add(new CounterViewModel(counter.Key, counter.Value.Count, counter.Value.Name));
                    }
                    else
                    {
                        viewModel.Name = counter.Value.Name;
                        viewModel.Count = counter.Value.Count;
                    }
                }

                foreach (var counter in counterState.Where(x => x.Value == null))
                {
                    var viewModel = Counters.FirstOrDefault(x => x.Id == counter.Key);
                    if (viewModel != null)
                    {
                        Counters.Remove(viewModel);
                    }
                }
            });

            lastSequenceSeen = maxSequence;
        }


        private class CounterState
        {
            public CounterState(string name, int count)
            {
                Name = name;
                Count = count;
            }

            public string Name { get; }
            public int Count { get; }
        }


        private CounterState ApplyEvent(CounterState state, CounterEvent counterEvent)
        {
            switch (counterEvent.EventCase)
            {
                case EventOneofCase.Added:
                    return new CounterState(counterEvent.Added.Name, 0);
                case EventOneofCase.Removed:
                    return null;
                case EventOneofCase.Incremented:
                    return new CounterState(state.Name, state.Count + 1);
                case EventOneofCase.Decremented:
                    return new CounterState(state.Name, state.Count - 1);
                case EventOneofCase.NameChanged:
                    return new CounterState(counterEvent.NameChanged.Name, state.Count);
                default:
                    throw new InvalidOperationException("Oh noes!");
            }
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

        private async void OnAddCounter()
        {
            var name = string.IsNullOrWhiteSpace(NewCounterName) ? Path.GetRandomFileName() : NewCounterName;
            var id = await counterApi.AddCounter(name).ConfigureAwait(true);
        }
    }
}