using System.Windows.Input;

namespace DumyWPF.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private ViewModelBase _currentViewModel;

        public ViewModelBase CurrentViewModel
        {
            get => _currentViewModel;
            set
            {
                _currentViewModel = value;
                OnPropertyChanged();
            }
        }

        // Команди навігації
        public ICommand NavigateDailyBalanceCommand { get; }
        public ICommand NavigateFocusModeCommand { get; }
        public ICommand NavigateTacticalModeCommand { get; }
        public ICommand NavigateHistoryCommand { get; }
        public ICommand NavigateSettingsCommand { get; }

        public MainViewModel()
        {
            // Старт з екрана онбордингу
            CurrentViewModel = new OnboardingViewModel(this);

            NavigateDailyBalanceCommand = new RelayCommand(_ => CurrentViewModel = new DailyBalanceViewModel());
            NavigateFocusModeCommand = new RelayCommand(_ => CurrentViewModel = new FocusModeViewModel());
            NavigateTacticalModeCommand = new RelayCommand(_ => CurrentViewModel = new TacticalModeViewModel());
            NavigateHistoryCommand = new RelayCommand(_ => CurrentViewModel = new HistoryViewModel());
            NavigateSettingsCommand = new RelayCommand(_ => CurrentViewModel = new SettingsViewModel());
        }
    }
}