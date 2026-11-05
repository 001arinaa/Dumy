using System.Windows.Input;

namespace DumyWPF.ViewModels
{
    /// <summary>Базовий клас екранів з навігацією (налаштування, історія, бібліотека, головна).</summary>
    public abstract class ScreenViewModelBase : ViewModelBase
    {
        protected MainViewModel Main { get; }

        public ICommand NavigateSettingsCommand { get; }
        public ICommand NavigateHistoryCommand { get; }
        public ICommand NavigateLibraryCommand { get; }
        public ICommand NavigateHomeCommand { get; }

        protected ScreenViewModelBase(MainViewModel main)
        {
            Main = main;
            NavigateSettingsCommand = new RelayCommand(_ => Main.ShowSettings());
            NavigateHistoryCommand = new RelayCommand(_ => Main.ShowHistory());
            NavigateLibraryCommand = new RelayCommand(_ => Main.ShowLibrary());
            NavigateHomeCommand = new RelayCommand(_ => Main.ShowHome());
        }
    }
}
