using System.Globalization;
using System.Windows.Media;
using DumyWPF.Models;

namespace DumyWPF.ViewModels
{
    /// <summary>Рядок історії: готовий до відображення запис MoodEntry.</summary>
    public class HistoryItemViewModel
    {
        public string DateText { get; }
        public string Emoji { get; }
        public string Title { get; }
        public string? Notes { get; }
        public Brush DotBrush { get; }

        public string Summary => Emoji + " " + Title;

        public HistoryItemViewModel(MoodEntry entry)
        {
            DateText = FormatDate(entry.Date);
            Notes = string.IsNullOrWhiteSpace(entry.Notes) ? null : entry.Notes;
            Emoji = MoodStates.EmojiFor(entry.EmotionState);

            string level = entry.Intensity + "/10";
            string title;
            string dot;

            switch (entry.EmotionState)
            {
                case MoodStates.Sos:
                    title = "SOS Заземлення";
                    dot = "#D9534F";
                    break;
                case MoodStates.TacticalState:
                    title = "Тактична фіксація стресу (" + level + ")";
                    dot = "#A3A075";
                    break;
                case MoodStates.Exhaustion:
                    title = "Рівень виснаження (" + level + ")";
                    dot = "#9AA3B6";
                    break;
                default:
                    title = "Настрій: " + entry.EmotionState + " (" + level + ")";
                    dot = "#DAB6C8";
                    break;
            }

            if (Notes != null)
            {
                title += " 📝";
            }

            Title = title;
            DotBrush = MakeBrush(dot);
        }

        private static string FormatDate(DateTime date)
        {
            var today = DateTime.Today;
            if (date.Date == today) return "Сьогодні, " + date.ToString("HH:mm");
            if (date.Date == today.AddDays(-1)) return "Вчора, " + date.ToString("HH:mm");
            return date.ToString("d MMMM", new CultureInfo("uk-UA"));
        }

        private static Brush MakeBrush(string hex)
        {
            var brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(hex)!);
            brush.Freeze();
            return brush;
        }
    }
}
