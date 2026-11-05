using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using DumyWPF.Models;
using DumyWPF.Services;

namespace DumyWPF.ViewModels
{
    /// <summary>Картка практики в бібліотеці.</summary>
    public class PracticeItemViewModel
    {
        private readonly Practice _practice;

        public string Title => _practice.Title;
        public string Instructions => _practice.Instructions;
        public string Meta => _practice.Type + " · " + _practice.DurationMin + " хв · " + _practice.ProfileTag;
        public bool HasVideo => !string.IsNullOrWhiteSpace(_practice.VideoUrl);
        public string PlayGlyph => HasVideo ? "▷" : "ℹ";
        public string VideoNote => HasVideo ? "Відео відкриється в браузері" : "Лише текстова інструкція";
        public string? VideoUrl => _practice.VideoUrl;

        public ICommand PlayCommand { get; }

        public PracticeItemViewModel(Practice practice)
        {
            _practice = practice;
            PlayCommand = new RelayCommand(_ => PracticeLauncher.Open(_practice));
        }
    }

    public class PracticesLibraryViewModel : ScreenViewModelBase
    {
        private const string AllProfiles = "Усі профілі";
        private const string AllStates = "Усі стани";

        private readonly PracticeService _practiceService = new();

        private string _selectedProfile = AllProfiles;
        private string _selectedState = AllStates;
        private string _countText = string.Empty;

        public ObservableCollection<PracticeItemViewModel> Practices { get; } = new();

        public List<string> ProfileFilters { get; } = new()
        {
            AllProfiles, AppProfiles.Daily, AppProfiles.Focus, AppProfiles.Tactical
        };

        public List<string> StateFilters { get; } = new()
        {
            AllStates, MoodStates.Anxious, MoodStates.Tired, MoodStates.Irritated, MoodStates.Calm
        };

        public string SelectedProfile
        {
            get => _selectedProfile;
            set
            {
                if (_selectedProfile == value || value == null) return;
                _selectedProfile = value;
                OnPropertyChanged();
                _ = RunSafeAsync(LoadAsync);
            }
        }

        public string SelectedState
        {
            get => _selectedState;
            set
            {
                if (_selectedState == value || value == null) return;
                _selectedState = value;
                OnPropertyChanged();
                _ = RunSafeAsync(LoadAsync);
            }
        }

        public string CountText
        {
            get => _countText;
            private set
            {
                _countText = value;
                OnPropertyChanged();
            }
        }

        public Visibility EmptyVisibility => Practices.Count == 0 ? Visibility.Visible : Visibility.Collapsed;

        public PracticesLibraryViewModel(MainViewModel main) : base(main)
        {
            _ = RunSafeAsync(LoadAsync);
        }

        private async Task LoadAsync()
        {
            bool byProfile = _selectedProfile != AllProfiles;
            bool byState = _selectedState != AllStates;

            List<Practice> list;
            if (byProfile && byState)
                list = await _practiceService.GetByProfileAndStateAsync(_selectedProfile, _selectedState);
            else if (byProfile)
                list = await _practiceService.GetByProfileTagAsync(_selectedProfile);
            else if (byState)
                list = await _practiceService.GetByTargetStateAsync(_selectedState);
            else
                list = await _practiceService.GetAllAsync();

            Practices.Clear();
            foreach (var practice in list)
            {
                Practices.Add(new PracticeItemViewModel(practice));
            }

            CountText = "Знайдено практик: " + list.Count;
            OnPropertyChanged(nameof(EmptyVisibility));
        }
    }
}
