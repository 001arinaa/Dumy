using System.Windows.Input;

namespace DumyWPF.ViewModels
{
    public class OnboardingViewModel : ViewModelBase
    {
        private string _userName = string.Empty;

        public string UserName
        {
            get => _userName;
            set
            {
                _userName = value;
                OnPropertyChanged();
            }
        }

        public ICommand StartCommand { get; }

        public OnboardingViewModel(MainViewModel mainViewModel)
        {
            StartCommand = new RelayCommand(_ =>
            {
                mainViewModel.CurrentViewModel = new DailyBalanceViewModel();
            });
        }
    }
}