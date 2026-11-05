using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Input;
using DumyWPF.Models;
using DumyWPF.Services;
using Microsoft.Win32;

namespace DumyWPF.ViewModels
{
    /// <summary>Один стовпчик діаграми тижня (висота в пікселях).</summary>
    public class ChartBarViewModel
    {
        public double BarHeight { get; }

        public ChartBarViewModel(double barHeight)
        {
            BarHeight = barHeight;
        }
    }

    public class HistoryViewModel : ScreenViewModelBase
    {
        private readonly MoodService _moodService = new();

        private string _averageText = "Середня інтенсивність: —";
        private string _totalText = "Всього записів: 0";
        private string _weekText = "За 7 днів: 0";
        private string _weekCountText = "Записів цього тижня: 0";
        private string _topStateText = "Найчастіший стан: —";

        public ObservableCollection<HistoryItemViewModel> Entries { get; } = new();
        public ObservableCollection<ChartBarViewModel> Bars { get; } = new();

        public ICommand ExportReportCommand { get; }

        public HistoryViewModel(MainViewModel main) : base(main)
        {
            ExportReportCommand = new RelayCommand(async _ => await RunSafeAsync(ExportAsync));
            _ = RunSafeAsync(LoadAsync);
        }

        public Visibility EmptyVisibility => Entries.Count == 0 ? Visibility.Visible : Visibility.Collapsed;

        public string AverageText
        {
            get => _averageText;
            private set { _averageText = value; OnPropertyChanged(); }
        }

        public string TotalText
        {
            get => _totalText;
            private set { _totalText = value; OnPropertyChanged(); }
        }

        public string WeekText
        {
            get => _weekText;
            private set { _weekText = value; OnPropertyChanged(); }
        }

        public string WeekCountText
        {
            get => _weekCountText;
            private set { _weekCountText = value; OnPropertyChanged(); }
        }

        public string TopStateText
        {
            get => _topStateText;
            private set { _topStateText = value; OnPropertyChanged(); }
        }

        private async Task LoadAsync()
        {
            var all = await _moodService.GetMoodHistoryAsync();
            var today = DateTime.Today;

            // Список останніх записів
            Entries.Clear();
            foreach (var entry in all.Take(10))
            {
                Entries.Add(new HistoryItemViewModel(entry));
            }
            OnPropertyChanged(nameof(EmptyVisibility));

            // Діаграма поточного тижня (середня інтенсивність за день)
            int diff = ((int)today.DayOfWeek + 6) % 7; // понеділок = 0
            var weekStart = today.AddDays(-diff);
            Bars.Clear();
            for (int i = 0; i < 7; i++)
            {
                var day = weekStart.AddDays(i);
                var items = all.Where(e => e.Date.Date == day).ToList();
                double height = items.Count > 0 ? Math.Max(8, items.Average(e => e.Intensity) * 11.5) : 4;
                Bars.Add(new ChartBarViewModel(height));
            }

            // Статистика
            var last7 = all.Where(e => e.Date >= today.AddDays(-6)).ToList();
            AverageText = last7.Count > 0
                ? "Середня інтенсивність: " + last7.Average(e => e.Intensity).ToString("0.0", CultureInfo.InvariantCulture)
                : "Середня інтенсивність: —";
            TotalText = "Всього записів: " + all.Count;
            WeekText = "За 7 днів: " + last7.Count;
            WeekCountText = "Записів цього тижня: " + all.Count(e => e.Date >= weekStart);

            var top = all.GroupBy(e => e.EmotionState)
                         .OrderByDescending(g => g.Count())
                         .FirstOrDefault();
            TopStateText = top != null ? "Найчастіший стан: " + top.Key : "Найчастіший стан: —";
        }

        private async Task ExportAsync()
        {
            var all = await _moodService.GetMoodHistoryAsync();

            var dialog = new SaveFileDialog
            {
                Title = "Експорт звіту",
                FileName = "dumy-report-" + DateTime.Now.ToString("yyyyMMdd") + ".csv",
                Filter = "CSV (*.csv)|*.csv"
            };

            if (dialog.ShowDialog() != true) return;

            var sb = new StringBuilder();
            sb.AppendLine("Дата;Стан;Інтенсивність;Нотатка");
            foreach (var e in all)
            {
                sb.AppendLine(e.Date.ToString("yyyy-MM-dd HH:mm") + ";" + Csv(e.EmotionState) + ";" + e.Intensity + ";" + Csv(e.Notes));
            }

            File.WriteAllText(dialog.FileName, sb.ToString(), new UTF8Encoding(true));
            MessageBox.Show("Звіт збережено:\n" + dialog.FileName, "Dumy",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private static string Csv(string? value)
        {
            if (string.IsNullOrEmpty(value)) return string.Empty;
            return "\"" + value.Replace("\"", "\"\"") + "\"";
        }
    }
}
