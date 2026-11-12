using System.Windows.Input;
using DumyWPF.Services;

namespace DumyWPF.ViewModels
{
    public class TacticalModeViewModel : ScreenViewModelBase
    {
        private readonly MoodService _moodService = new();
        private readonly PracticeService _practiceService = new();

        private double _stress = 7;
        private string _statusMessage = string.Empty;

        public ICommand SosClickCommand { get; }
        public ICommand SaveTacticalStateCommand { get; }
        public ICommand PlayBreathingCommand { get; }
        public ICommand PlayGroundingCommand { get; }

        public TacticalModeViewModel(MainViewModel main) : base(main)
        {
            SosClickCommand = new RelayCommand(async _ => await RunSafeAsync(SosAsync));
            SaveTacticalStateCommand = new RelayCommand(async _ => await RunSafeAsync(SaveStateAsync));
            PlayBreathingCommand = new RelayCommand(async _ => await RunSafeAsync(() => PlayAsync(PracticeTitles.TacticalBreathing)));
            PlayGroundingCommand = new RelayCommand(async _ => await RunSafeAsync(() => PlayAsync(PracticeTitles.Grounding)));
        }

        public double StressLevel
        {
            get => _stress;
            set
            {
                _stress = Math.Round(value);
                OnPropertyChanged();
                OnPropertyChanged(nameof(StressText));
            }
        }

        private int StressValue => (int)Math.Round(_stress);

        public string StressText
        {
            get
            {
                int n = StressValue;
                string label = n <= 3 ? "Низька напруга" : n <= 6 ? "Помірна напруга" : n <= 8 ? "Висока напруга" : "Критична напруга";
                return label + " (" + n + " з 10)";
            }
        }

        public string StatusMessage
        {
            get => _statusMessage;
            private set
            {
                _statusMessage = value;
                OnPropertyChanged();
            }
        }

        /// <summary>SOS: миттєво фіксує тактичний зріз у БД і показує протокол заземлення.</summary>
        private async Task SosAsync()
        {
            await _moodService.SaveMoodAsync(MoodStates.Sos, StressValue, "Швидке SOS-заземлення");
            StatusMessage = "✓ SOS зафіксовано в історії";

            // Окреме вікно з анімацією дихання
            var window = new Views.SosWindow { Owner = System.Windows.Application.Current.MainWindow };
            window.ShowDialog();
        }

        /// <summary>Експрес-фіксація стану (слайдер стресу).</summary>
        private async Task SaveStateAsync()
        {
            await _moodService.SaveMoodAsync(MoodStates.TacticalState, StressValue);
            StatusMessage = "✓ Стан збережено: " + StressValue + " з 10";
        }

        private async Task PlayAsync(string title)
        {
            var practice = await _practiceService.GetByTitleAsync(title);
            PracticeLauncher.Open(practice);
        }
    }
}
