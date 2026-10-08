using System.Windows.Input;

namespace DumyWPF.ViewModels
{
    public class DailyBalanceViewModel : ViewModelBase
    {
        private readonly MainViewModel? _mainViewModel;

        public ICommand? NavigateSettingsCommand { get; }
        public ICommand? NavigateHistoryCommand { get; }

        public DailyBalanceViewModel(MainViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;

            NavigateSettingsCommand = new RelayCommand(_ =>
            {
                if (_mainViewModel != null)
                    _mainViewModel.CurrentViewModel = new SettingsViewModel();
            });

            NavigateHistoryCommand = new RelayCommand(_ =>
            {
                if (_mainViewModel != null)
                    _mainViewModel.CurrentViewModel = new HistoryViewModel();
            });
        }

        // Пустий конструктор для дизайнера або спрощеного ініціалізування
        public DailyBalanceViewModel()
        {
        }
    }
}