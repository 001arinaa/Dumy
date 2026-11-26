using DumyWPF.Data;
using DumyWPF.Services;
using DumyWPF.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace DumyWPF.Tests
{
    /// <summary>Зв'язок «збереження звичок ↔ їх відображення».</summary>
    [Collection("Database")]
    public class HabitsViewModelTests : DatabaseTestBase
    {
        private readonly HabitService _service = new();

        private static async Task<int> TotalRowsAsync()
        {
            using var db = new AppDbContext();
            return await db.HabitLogs.CountAsync();
        }

        [Fact]
        public void NewHabits_HaveGoalsAndAreNotCompleted()
        {
            var habits = new HabitsViewModel();

            Assert.Equal("Ціль: 2.0 л", habits.Water.Subtitle);
            Assert.Equal("Ціль: 8.0 год", habits.Sleep.Subtitle);
            Assert.Equal("Ціль: 1 сесія", habits.Focus.Subtitle);
            Assert.False(habits.Water.IsCompleted);
            Assert.False(habits.Sleep.IsCompleted);
            Assert.False(habits.Focus.IsCompleted);
        }

        [Fact]
        public void Subtitle_ChangesWhenCompleted_AndRaisesNotification()
        {
            var habit = new HabitItemViewModel("Вода", "Ціль: 2.0 л", _service);
            habit.SetLoaded(false);
            var changed = new List<string?>();
            habit.PropertyChanged += (_, e) => changed.Add(e.PropertyName);

            habit.SetLoaded(true);

            Assert.Equal("Виконано сьогодні", habit.Subtitle);
            Assert.Contains(nameof(HabitItemViewModel.IsCompleted), changed);
            Assert.Contains(nameof(HabitItemViewModel.Subtitle), changed);
        }

        [Fact]
        public async Task SetLoaded_DoesNotWriteToDatabase()
        {
            var habit = new HabitItemViewModel("Вода", "Ціль: 2.0 л", _service);

            habit.SetLoaded(true);
            await Task.Delay(300);

            Assert.Equal(0, await TotalRowsAsync());
        }

        [Fact]
        public async Task ClickingCheckbox_SavesToDatabase()
        {
            var habit = new HabitItemViewModel("Вода", "Ціль: 2.0 л", _service);

            habit.IsCompleted = true;

            Assert.True(await WaitUntilAsync(async () =>
                (await _service.GetTodayAsync()).GetValueOrDefault("Вода")));
        }

        [Fact]
        public async Task LoadAsync_ShowsSavedHabits_WithoutRewritingThem()
        {
            await _service.SetAsync("Вода", true);
            await _service.SetAsync("Сон", false);
            var habits = new HabitsViewModel();

            await habits.LoadAsync();
            await Task.Delay(300);

            Assert.True(habits.Water.IsCompleted);
            Assert.Equal("Виконано сьогодні", habits.Water.Subtitle);
            Assert.False(habits.Sleep.IsCompleted);
            Assert.Equal("Ціль: 8.0 год", habits.Sleep.Subtitle);
            Assert.False(habits.Focus.IsCompleted);
            Assert.Equal(2, await TotalRowsAsync());
        }

        [Fact]
        public async Task SavedOnOneScreen_IsShownOnAnotherScreen()
        {
            // «Щоденний баланс» ставить галочки...
            var daily = new HabitsViewModel();
            daily.Water.IsCompleted = true;
            daily.Focus.IsCompleted = true;
            Assert.True(await WaitUntilAsync(async () =>
            {
                var t = await _service.GetTodayAsync();
                return t.GetValueOrDefault("Вода") && t.GetValueOrDefault("Фокус");
            }));

            // ...а «Фокус та Вигорання» (новий екран, нові об'єкти) їх показує.
            var focus = new HabitsViewModel();
            await focus.LoadAsync();

            Assert.True(focus.Water.IsCompleted);
            Assert.True(focus.Focus.IsCompleted);
            Assert.False(focus.Sleep.IsCompleted);
        }

        [Fact]
        public async Task UncheckingHabit_IsAlsoPersisted()
        {
            var habits = new HabitsViewModel();
            habits.Sleep.IsCompleted = true;
            Assert.True(await WaitUntilAsync(async () => (await _service.GetTodayAsync()).GetValueOrDefault("Сон")));

            habits.Sleep.IsCompleted = false;
            Assert.True(await WaitUntilAsync(async () =>
                (await _service.GetTodayAsync()).TryGetValue("Сон", out var v) && !v));

            var reloaded = new HabitsViewModel();
            await reloaded.LoadAsync();
            Assert.False(reloaded.Sleep.IsCompleted);
        }
    }
}
