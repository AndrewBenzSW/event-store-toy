using CounterService;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Threading;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;

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
                    new AutomatedCounter(Guid.NewGuid()),
                    new AutomatedCounter(Guid.NewGuid()),
                    new AutomatedCounter(Guid.NewGuid()),
                    new AutomatedCounter(Guid.NewGuid()),
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
                switch (counterEvent.Value.EventType)
                {
                    case CounterEvents.CounterAdded:
                        await DispatcherHelper.RunAsync(() => Counters.Add(new AutomatedCounter(counterEvent.Value.CounterId)));
                        break;
                    case CounterEvents.CounterRemoved:
                        var counterToRemove = Counters.SingleOrDefault(x => x.Id == counterEvent.Value.CounterId);
                        if (counterToRemove != null)
                        {
                            await DispatcherHelper.RunAsync(() => Counters.Remove(counterToRemove));
                        }
                        break;
                }

                lastSequence = counterEvent.Key;
            }
        }
    }
}