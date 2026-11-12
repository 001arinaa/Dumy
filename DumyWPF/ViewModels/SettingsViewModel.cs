using System.IO;
using System.Windows;
using System.Windows.Input;
using DumyWPF.Data;
using DumyWPF.Services;
using Microsoft.Data.Sqlite;
using Microsoft.Win32;

namespace DumyWPF.ViewModels
{
    public class SettingsViewModel : ScreenViewModelBase
    {
        private readonly MoodService _moodService = new();
        private readonly HabitService _habitService = new();

        private string _profile;
        private string _statusMessage;

        public string StatusMessage
        {
            get => _statusMessage;
            set { _statusMessage = value; OnPropertyChanged(); }
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

        public ICommand ClearHistoryCommand { get; }
        public ICommand BackupDataCommand { get; }
        public ICommand CancelSettingsCommand { get; }
        public ICommand SaveSettingsCommand { get; }

        public SettingsViewModel(MainViewModel main) : base(main)
        {
            _profile = main.CurrentProfile;

            CancelSettingsCommand = new RelayCommand(_ => Main.ShowHome());
            SaveSettingsCommand = new RelayCommand(_ =>
            {
                Main.CurrentProfile = _profile;
                Main.ShowHome();
            });
            ClearHistoryCommand = new RelayCommand(async _ => await RunSafeAsync(ClearHistoryAsync));
            BackupDataCommand = new RelayCommand(async _ => await RunSafeAsync(BackupAsync));
        }

        private void SelectProfile(string profile)
        {
            if (_profile == profile) return;
            _profile = profile;
            OnPropertyChanged(nameof(IsDailyMode));
            OnPropertyChanged(nameof(IsFocusMode));
            OnPropertyChanged(nameof(IsTacticalMode));
        }

        private async Task ClearHistoryAsync()
        {
            var answer = MessageBox.Show(
                "Видалити всю локальну історію настрою та відмітки звичок? Цю дію не можна скасувати.",
                "Очистити історію", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (answer != MessageBoxResult.Yes) return;

            await _moodService.ClearHistoryAsync();
            await _habitService.ClearAllAsync();

            // Виводимо статус дрібним текстом замість модального вікна
            StatusMessage = "✓ Історію успішно очищено.";
        }

        private Task BackupAsync()
        {
            var dialog = new SaveFileDialog
            {
                Title = "Резервна копія бази даних",
                FileName = "dumy-backup-" + DateTime.Now.ToString("yyyyMMdd") + ".db",
                Filter = "База даних SQLite (*.db)|*.db"
            };

            if (dialog.ShowDialog() == true)
            {
                SqliteConnection.ClearAllPools(); // відпускаємо файл бази перед копіюванням
                File.Copy(AppDbContext.DbPath, dialog.FileName, true);

                // Виводимо шлях резервної копії як статусний текст
                StatusMessage = "✓ Резервну копію збережено: " + Path.GetFileName(dialog.FileName);
            }

            return Task.CompletedTask;
        }
    }
}