using EventStoreToy.CounterService;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.Windows.Input;

namespace EventStoreToy.ViewModel
{
    public class CounterViewModel : ViewModelBase
    {
        private readonly CounterApi counterApi;
        private int count;

        public CounterViewModel(Guid id, int count)
        {

            if (!IsInDesignMode)
            {
                counterApi = new CounterApi();
                Remove = new RelayCommand(OnRemove);
                Increment = new RelayCommand(OnIncrement);
                Decrement = new RelayCommand(OnDecrement);
            }

            Id = id;
            Count = count;
        }

        public Guid Id { get; }

        public int Count
        {
            get => count;
            set => Set(ref count, value);
        }

        public ICommand Remove { get; }

        public ICommand Increment { get; }

        public ICommand Decrement { get; }

        private void OnRemove()
        {
            counterApi.RemoveCounter(Id);
        }

        private void OnIncrement()
        {
            counterApi.Increment(Id);
        }

        private void OnDecrement()
        {
            counterApi.Decrement(Id);
        }
    }
}