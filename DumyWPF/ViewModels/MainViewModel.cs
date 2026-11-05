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

        /// <summary>Ім'я користувача з онбордингу.</summary>
        public string UserName { get; set; } = string.Empty;

        /// <summary>Обраний профіль (визначає, який екран є «головним»).</summary>
        public string CurrentProfile { get; set; } = AppProfiles.Daily;

        // Команди навігації
        public ICommand NavigateDailyBalanceCommand { get; }
        public ICommand NavigateFocusModeCommand { get; }
        public ICommand NavigateTacticalModeCommand { get; }
        public ICommand NavigateHistoryCommand { get; }
        public ICommand NavigateSettingsCommand { get; }
        public ICommand NavigateLibraryCommand { get; }

        public MainViewModel()
        {
            // Старт з екрана онбордингу
            _currentViewModel = new OnboardingViewModel(this);

            NavigateDailyBalanceCommand = new RelayCommand(_ => ShowDailyBalance());
            NavigateFocusModeCommand = new RelayCommand(_ => ShowFocusMode());
            NavigateTacticalModeCommand = new RelayCommand(_ => ShowTacticalMode());
            NavigateHistoryCommand = new RelayCommand(_ => ShowHistory());
            NavigateSettingsCommand = new RelayCommand(_ => ShowSettings());
            NavigateLibraryCommand = new RelayCommand(_ => ShowLibrary());
        }

        public void ShowDailyBalance() => CurrentViewModel = new DailyBalanceViewModel(this);
        public void ShowFocusMode() => CurrentViewModel = new FocusModeViewModel(this);
        public void ShowTacticalMode() => CurrentViewModel = new TacticalModeViewModel(this);
        public void ShowHistory() => CurrentViewModel = new HistoryViewModel(this);
        public void ShowSettings() => CurrentViewModel = new SettingsViewModel(this);
        public void ShowLibrary() => CurrentViewModel = new PracticesLibraryViewModel(this);

        /// <summary>Головний екран залежно від обраного профілю.</summary>
        public void ShowHome()
        {
            switch (CurrentProfile)
            {
                case AppProfiles.Focus:
                    ShowFocusMode();
                    break;
                case AppProfiles.Tactical:
                    ShowTacticalMode();
                    break;
                default:
                    ShowDailyBalance();
                    break;
            }
        }
    }
}
