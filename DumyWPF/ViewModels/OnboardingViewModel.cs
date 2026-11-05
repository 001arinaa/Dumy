using System.Windows.Input;

namespace DumyWPF.ViewModels
{
    public class OnboardingViewModel : ViewModelBase
    {
        private readonly MainViewModel _main;
        private string _userName = string.Empty;
        private string _profile = AppProfiles.Daily;

        public string UserName
        {
            get => _userName;
            set
            {
                _userName = value;
                OnPropertyChanged();
            }
        }

        public bool IsDailyMode
        {
            get => _profile == AppProfiles.Daily;
            set { if (value) SelectProfile(AppProfiles.Daily); }
        }

        public bool IsFocusMode
        {
            get => _profile == AppProfiles.Focus;
            set { if (value) SelectProfile(AppProfiles.Focus); }
        }

        public bool IsTacticalMode
        {
            get => _profile == AppProfiles.Tactical;
            set { if (value) SelectProfile(AppProfiles.Tactical); }
        }

        public ICommand StartCommand { get; }
        public ICommand SelectDailyCommand { get; }
        public ICommand SelectFocusCommand { get; }
        public ICommand SelectTacticalCommand { get; }

        public OnboardingViewModel(MainViewModel mainViewModel)
        {
            _main = mainViewModel;

            SelectDailyCommand = new RelayCommand(_ => SelectProfile(AppProfiles.Daily));
            SelectFocusCommand = new RelayCommand(_ => SelectProfile(AppProfiles.Focus));
            SelectTacticalCommand = new RelayCommand(_ => SelectProfile(AppProfiles.Tactical));

            StartCommand = new RelayCommand(_ =>
            {
                _main.UserName = UserName.Trim();
                _main.CurrentProfile = _profile;
                _main.ShowHome();
            });
        }

        private void SelectProfile(string profile)
        {
            if (_profile == profile) return;
            _profile = profile;
            OnPropertyChanged(nameof(IsDailyMode));
            OnPropertyChanged(nameof(IsFocusMode));
            OnPropertyChanged(nameof(IsTacticalMode));
        }
    }
}
