using System.Windows.Input;
using DumyWPF.Models;
using DumyWPF.Services;

namespace DumyWPF.ViewModels
{
    public class DailyBalanceViewModel : ScreenViewModelBase
    {
        private readonly MoodService _moodService = new();
        private readonly PracticeService _practiceService = new();
        private readonly Random _random = new();

        private string _emotion = MoodStates.Calm;
        private double _intensity = 5;
        private string _notes = string.Empty;
        private string _statusMessage = string.Empty;
        private string _weekEntriesText = "Збережено записів цього тижня: 0";
        private Practice? _recommended;

        public HabitsViewModel Habits { get; } = new();

        public ICommand SaveMoodCommand { get; }
        public ICommand StartRecommendedCommand { get; }

        public DailyBalanceViewModel(MainViewModel main) : base(main)
        {
            SaveMoodCommand = new RelayCommand(async _ => await RunSafeAsync(SaveMoodAsync));
            StartRecommendedCommand = new RelayCommand(_ => PracticeLauncher.Open(_recommended));

            _ = RunSafeAsync(InitializeAsync);
        }

        // ---- Вибір стану (4 картки-RadioButton) ----
        public bool IsCalm
        {
            get => _emotion == MoodStates.Calm;
            set { if (value) SelectEmotion(MoodStates.Calm); }
        }

        public bool IsAnxious
        {
            get => _emotion == MoodStates.Anxious;
            set { if (value) SelectEmotion(MoodStates.Anxious); }
        }

        public bool IsTired
        {
            get => _emotion == MoodStates.Tired;
            set { if (value) SelectEmotion(MoodStates.Tired); }
        }

        public bool IsIrritated
        {
            get => _emotion == MoodStates.Irritated;
            set { if (value) SelectEmotion(MoodStates.Irritated); }
        }

        // ---- Інтенсивність ----
        public double Intensity
        {
            get => _intensity;
            set
            {
                _intensity = Math.Round(value);
                OnPropertyChanged();
                OnPropertyChanged(nameof(IntensityText));
            }
        }

        private int IntensityValue => (int)Math.Round(_intensity);

        public string IntensityText
        {
            get
            {
                int n = IntensityValue;
                string label = n <= 3 ? "Низько" : n <= 6 ? "Помірно" : n <= 8 ? "Високо" : "Дуже високо";
                return label + " (" + n + " з 10)";
            }
        }

        // ---- Щоденник думок ----
        public string Notes
        {
            get => _notes;
            set
            {
                _notes = value ?? string.Empty;
                OnPropertyChanged();
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

        public string WeekEntriesText
        {
            get => _weekEntriesText;
            private set
            {
                _weekEntriesText = value;
                OnPropertyChanged();
            }
        }

        // ---- Рекомендація ----
        public string RecommendedTitle =>
            _recommended?.Title ?? "Оберіть стан, щоб отримати рекомендацію";

        public string RecommendedSummary =>
            _recommended == null ? string.Empty : "Перший крок: " + FirstStep(_recommended.Instructions);

        public string RecommendedDuration =>
            _recommended == null ? string.Empty : "Тривалість: " + _recommended.DurationMin + " хв";

        public string RecommendedType =>
            _recommended == null ? string.Empty : "Тип: " + _recommended.Type;

        // ---- Логіка ----
        private async Task InitializeAsync()
        {
            await Habits.LoadAsync();
            await RefreshWeekCountAsync();
            await UpdateRecommendationAsync();
        }

        private void SelectEmotion(string emotion)
        {
            if (_emotion == emotion) return;
            _emotion = emotion;
            OnPropertyChanged(nameof(IsCalm));
            OnPropertyChanged(nameof(IsAnxious));
            OnPropertyChanged(nameof(IsTired));
            OnPropertyChanged(nameof(IsIrritated));
            _ = RunSafeAsync(UpdateRecommendationAsync);
        }

        private async Task SaveMoodAsync()
        {
            string? notes = string.IsNullOrWhiteSpace(_notes) ? null : _notes.Trim();

            await _moodService.SaveMoodAsync(_emotion, IntensityValue, notes);

            StatusMessage = "✓ Збережено: " + _emotion + " (" + IntensityValue + " з 10)";
            Notes = string.Empty;

            await RefreshWeekCountAsync();
            await UpdateRecommendationAsync();
        }

        private async Task RefreshWeekCountAsync()
        {
            var today = DateTime.Today;
            int diff = ((int)today.DayOfWeek + 6) % 7; // понеділок = 0
            var weekStart = today.AddDays(-diff);
            int count = await _moodService.CountSinceAsync(weekStart);
            WeekEntriesText = "Збережено записів цього тижня: " + count;
        }

        private async Task UpdateRecommendationAsync()
        {
            var list = await _practiceService.GetByTargetStateAsync(_emotion);
            var preferred = list.Where(p => p.ProfileTag == AppProfiles.Daily).ToList();
            var pool = preferred.Count > 0 ? preferred : list;

            _recommended = pool.Count > 0 ? pool[_random.Next(pool.Count)] : null;

            OnPropertyChanged(nameof(RecommendedTitle));
            OnPropertyChanged(nameof(RecommendedSummary));
            OnPropertyChanged(nameof(RecommendedDuration));
            OnPropertyChanged(nameof(RecommendedType));
        }

        private static string FirstStep(string instructions)
        {
            string line = instructions.Split('\n')[0];
            int dot = line.IndexOf(". ", StringComparison.Ordinal);
            return dot >= 0 && dot < 3 ? line.Substring(dot + 2) : line;
        }
    }
}
