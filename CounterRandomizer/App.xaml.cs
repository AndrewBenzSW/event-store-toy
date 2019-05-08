using GalaSoft.MvvmLight.Threading;
using System.Windows;

namespace CounterRandomizer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            DispatcherHelper.Initialize();
        }
    }
}
