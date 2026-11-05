using System.Windows;
using DumyWPF.Services;

namespace DumyWPF.ViewModels
{
    /// <summary>Одна швидка звичка (Вода / Сон / Фокус). Стан зберігається в БД при кожному кліку.</summary>
    public class HabitItemViewModel : ViewModelBase
    {
        private readonly HabitService _service;
        private bool _isCompleted;
        private bool _isLoading;

        public string Name { get; }
        public string Goal { get; }

        public bool IsCompleted
        {
            get => _isCompleted;
            set
            {
                if (_isCompleted == value) return;
                _isCompleted = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Subtitle));
                if (!_isLoading)
                {
                    _ = SaveAsync();
                }
            }
        }

        public string Subtitle => _isCompleted ? "Виконано сьогодні" : Goal;

        public HabitItemViewModel(string name, string goal, HabitService service)
        {
            Name = name;
            Goal = goal;
            _service = service;
        }

        /// <summary>Встановлює стан, завантажений з БД (без повторного збереження).</summary>
        public void SetLoaded(bool value)
        {
            _isLoading = true;
            IsCompleted = value;
            _isLoading = false;
        }

        private async Task SaveAsync()
        {
            try
            {
                await _service.SetAsync(Name, _isCompleted);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Не вдалося зберегти звичку:\n" + ex.Message, "Dumy",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
