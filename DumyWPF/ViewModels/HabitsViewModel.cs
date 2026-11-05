using DumyWPF.Services;

namespace DumyWPF.ViewModels
{
    /// <summary>Три швидкі звички, спільні для екранів «Щоденний баланс» і «Фокус та Вигорання».</summary>
    public class HabitsViewModel : ViewModelBase
    {
        private readonly HabitService _service = new();

        public HabitItemViewModel Water { get; }
        public HabitItemViewModel Sleep { get; }
        public HabitItemViewModel Focus { get; }

        public HabitsViewModel()
        {
            Water = new HabitItemViewModel("Вода", "Ціль: 2.0 л", _service);
            Sleep = new HabitItemViewModel("Сон", "Ціль: 8.0 год", _service);
            Focus = new HabitItemViewModel("Фокус", "Ціль: 1 сесія", _service);
        }

        public async Task LoadAsync()
        {
            var today = await _service.GetTodayAsync();
            Water.SetLoaded(today.GetValueOrDefault(Water.Name));
            Sleep.SetLoaded(today.GetValueOrDefault(Sleep.Name));
            Focus.SetLoaded(today.GetValueOrDefault(Focus.Name));
        }
    }
}
