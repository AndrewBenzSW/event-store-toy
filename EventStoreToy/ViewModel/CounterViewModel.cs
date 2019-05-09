using CounterService;
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
        private string name;
        private bool nameChanging;
        private string temporaryName;

        public CounterViewModel(Guid id, int count, string name)
        {

            if (!IsInDesignMode)
            {
                counterApi = new CounterApi();
                Remove = new RelayCommand(OnRemove);
                Increment = new RelayCommand(OnIncrement);
                Decrement = new RelayCommand(OnDecrement);
                ChangeName = new RelayCommand(OnChangeName);
                SaveNameChange = new RelayCommand(OnSaveNameChange);
                CancelNameChange = new RelayCommand(OnCancelNameChange);
            }

            Id = id;
            Count = count;
            Name = name;
        }

        public Guid Id { get; }

        public int Count
        {
            get => count;
            set => Set(ref count, value);
        }

        public string Name
        {
            get => name;
            set => Set(ref name, value);
        }

        public string TemporaryName
        {
            get => temporaryName;
            set => Set(ref temporaryName, value);
        }

        public bool NameChanging
        {
            get => nameChanging;
            set => Set(ref nameChanging, value);
        }

        public ICommand Remove { get; }
        public ICommand Increment { get; }
        public ICommand Decrement { get; }
        public ICommand ChangeName { get; }
        public ICommand SaveNameChange { get; }
        public ICommand CancelNameChange { get; }

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

        private void OnChangeName()
        {
            TemporaryName = Name;
            NameChanging = true;
        }

        private void OnCancelNameChange()
        {
            NameChanging = false;
        }

        private void OnSaveNameChange()
        {
            counterApi.ChangeName(Id, TemporaryName, Name);
            NameChanging = false;
        }
    }
}