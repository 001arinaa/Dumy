using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using DumyWPF.Services;

namespace DumyWPF.ViewModels
{
    public class FocusModeViewModel : ScreenViewModelBase
    {
        private readonly MoodService _moodService = new();
        private readonly PracticeService _practiceService = new();

        private double _fatigue = 6;
        private string _statusMessage = string.Empty;

        public HabitsViewModel Habits { get; } = new();

        /// <summary>Останні записи («Остання активність»).</summary>
        public ObservableCollection<HistoryItemViewModel> Recent { get; } = new();

        /// <summary>Підказка «Поки що немає записів» показується, поки список порожній.</summary>
        public Visibility EmptyVisibility => Recent.Count == 0 ? Visibility.Visible : Visibility.Collapsed;

        public ICommand LogFatigueCommand { get; }
        public ICommand PlayEyesCommand { get; }
        public ICommand PlayBreathingCommand { get; }

        public FocusModeViewModel(MainViewModel main) : base(main)
        {
            LogFatigueCommand = new RelayCommand(async _ => await RunSafeAsync(LogFatigueAsync));
            PlayEyesCommand = new RelayCommand(async _ => await RunSafeAsync(() => PlayAsync(PracticeTitles.EyeExercise)));
            PlayBreathingCommand = new RelayCommand(async _ => await RunSafeAsync(() => PlayAsync(PracticeTitles.Breathing478)));

            _ = RunSafeAsync(InitializeAsync);
        }

        public double FatigueLevel
        {
            get => _fatigue;
            set
            {
                _fatigue = Math.Round(value);
                OnPropertyChanged();
                OnPropertyChanged(nameof(FatigueText));
            }
        }

        private int FatigueValue => (int)Math.Round(_fatigue);

        public string FatigueText
        {
            get
            {
                int n = FatigueValue;
                string label = n <= 3 ? "Легка втома" : n <= 6 ? "Помірна втома" : n <= 8 ? "Сильна втома" : "Виснаження";
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

        private async Task InitializeAsync()
        {
            await Habits.LoadAsync();
            await RefreshRecentAsync();
        }

        private async Task LogFatigueAsync()
        {
            await _moodService.SaveMoodAsync(MoodStates.Exhaustion, FatigueValue);
            StatusMessage = "✓ Рівень виснаження збережено: " + FatigueValue + " з 10";
            await RefreshRecentAsync();
        }

        private async Task RefreshRecentAsync()
        {
            var history = await _moodService.GetMoodHistoryAsync();
            Recent.Clear();
            foreach (var entry in history.Take(3))
            {
                Recent.Add(new HistoryItemViewModel(entry));
            }
            OnPropertyChanged(nameof(EmptyVisibility));
        }

        private async Task PlayAsync(string title)
        {
            var practice = await _practiceService.GetByTitleAsync(title);
            PracticeLauncher.Open(practice);
        }
    }
}
