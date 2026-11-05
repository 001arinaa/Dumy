using System.Diagnostics;
using System.Windows;
using DumyWPF.Models;

namespace DumyWPF.ViewModels
{
    /// <summary>
    /// Відкриває практику: якщо є VideoUrl — відео в браузері,
    /// якщо відео немає — показує текстову інструкцію.
    /// </summary>
    public static class PracticeLauncher
    {
        public static void Open(Practice? practice)
        {
            if (practice == null)
            {
                MessageBox.Show("Практику не знайдено в базі даних.", "Dumy",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (!string.IsNullOrWhiteSpace(practice.VideoUrl))
            {
                try
                {
                    Process.Start(new ProcessStartInfo(practice.VideoUrl) { UseShellExecute = true });
                    return;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Не вдалося відкрити відео:\n" + ex.Message, "Dumy",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }

            MessageBox.Show(practice.Instructions,
                $"{practice.Title} · {practice.DurationMin} хв",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
