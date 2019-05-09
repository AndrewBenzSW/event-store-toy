using CounterService;
using CounterService.Extensions;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Threading;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using static CounterEvent;

namespace CounterRandomizer.ViewModel
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
        private readonly Timer timer;
        private long lastSequence;

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            if (IsInDesignMode)
            {
                // Code runs in Blend --> create design time data.
                Counters = new ObservableCollection<AutomatedCounter>(new List<AutomatedCounter>
                {
                    new AutomatedCounter(Guid.NewGuid(), "Foo"),
                    new AutomatedCounter(Guid.NewGuid(), "Bar"),
                    new AutomatedCounter(Guid.NewGuid(), null),
                    new AutomatedCounter(Guid.NewGuid(), "Quux"),
                });
            }
            else
            {
                // Code runs "for real"
                timer = new Timer(OnTimerTick, null, 0, Timeout.Infinite);
            }
        }

        public ObservableCollection<AutomatedCounter> Counters { get; } = new ObservableCollection<AutomatedCounter>();

        private async void OnTimerTick(object state)
        {
            try
            {
                await UpdateState().ConfigureAwait(false);
            }
            finally
            {
                timer.Change(TimeSpan.FromSeconds(5), Timeout.InfiniteTimeSpan);
            }
        }

        private async System.Threading.Tasks.Task UpdateState()
        {
            var events = await CounterStore.AllEventsAfter(lastSequence).ConfigureAwait(false);

            foreach (var counterEvent in events)
            {
                switch (counterEvent.Value.EventCase)
                {
                    case EventOneofCase.Added:
                        await DispatcherHelper.RunAsync(() => Counters.Add(new AutomatedCounter(counterEvent.Value.Id.ToGuid(), counterEvent.Value.Added.Name)));
                        break;
                    case EventOneofCase.Removed:
                        {
                            var counterId = counterEvent.Value.Id.ToGuid();
                            var counterToRemove = Counters.SingleOrDefault(x => x.Id == counterId);
                            if (counterToRemove != null)
                            {
                                await DispatcherHelper.RunAsync(() => Counters.Remove(counterToRemove));
                            }
                        }

                        break;
                    case EventOneofCase.NameChanged:
                        {
                            var counterId = counterEvent.Value.Id.ToGuid();
                            var counterToUpdate = Counters.SingleOrDefault(x => x.Id == counterId);
                            if (counterToUpdate != null)
                            {
                                await DispatcherHelper.RunAsync(() => counterToUpdate.Name = counterEvent.Value.NameChanged.Name);
                            }
                        }

                        break;
                }

                lastSequence = counterEvent.Key;
            }
        }
    }
}