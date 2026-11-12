using DumyWPF.Services;
using DumyWPF.ViewModels;

namespace DumyWPF.Tests
{
    [Collection("Database")]
    public class OnboardingAndNavigationTests : DatabaseTestBase
    {
        [Fact]
        public void MainViewModel_StartsOnOnboarding()
        {
            var main = new MainViewModel();

            Assert.IsType<OnboardingViewModel>(main.CurrentViewModel);
        }

        [Fact]
        public void Onboarding_NoModeSelectedAtStart()
        {
            var vm = new OnboardingViewModel(new MainViewModel());

            Assert.False(vm.IsDailyMode);
            Assert.False(vm.IsFocusMode);
            Assert.False(vm.IsTacticalMode);
        }

        [Fact]
        public void Onboarding_SelectingModeIsExclusive_AndNotifiesView()
        {
            var vm = new OnboardingViewModel(new MainViewModel());
            var changed = new List<string?>();
            vm.PropertyChanged += (_, e) => changed.Add(e.PropertyName);

            vm.SelectFocusCommand.Execute(null);
            Assert.True(vm.IsFocusMode);
            Assert.False(vm.IsDailyMode);

            vm.SelectTacticalCommand.Execute(null);
            Assert.True(vm.IsTacticalMode);
            Assert.False(vm.IsFocusMode);

            Assert.Contains(nameof(OnboardingViewModel.IsFocusMode), changed);
            Assert.Contains(nameof(OnboardingViewModel.IsTacticalMode), changed);
        }

        [Fact]
        public void Onboarding_UncheckingRadioButton_DoesNotChangeSelection()
        {
            var vm = new OnboardingViewModel(new MainViewModel());
            vm.IsDailyMode = true;

            vm.IsDailyMode = false;

            Assert.True(vm.IsDailyMode);
        }

        [Fact]
        public async Task Start_SavesTrimmedNameAndOpensChosenScreen()
        {
            var main = new MainViewModel();
            var vm = new OnboardingViewModel(main) { UserName = "  Дана  " };
            vm.SelectFocusCommand.Execute(null);

            vm.StartCommand.Execute(null);

            Assert.Equal("Дана", main.UserName);
            Assert.Equal(AppProfiles.Focus, main.CurrentProfile);
            Assert.IsType<FocusModeViewModel>(main.CurrentViewModel);
            await Task.Delay(300); // дати екрану дочитати дані з БД
        }

        [Theory]
        [InlineData(AppProfiles.Daily, typeof(DailyBalanceViewModel))]
        [InlineData(AppProfiles.Focus, typeof(FocusModeViewModel))]
        [InlineData(AppProfiles.Tactical, typeof(TacticalModeViewModel))]
        public async Task ShowHome_OpensScreenOfCurrentProfile(string profile, Type expected)
        {
            var main = new MainViewModel { CurrentProfile = profile };

            main.ShowHome();

            Assert.IsType(expected, main.CurrentViewModel);
            await Task.Delay(300);
        }

        [Fact]
        public async Task ShowFocusMode_CreatesNewViewModelEachTime()
        {
            var main = new MainViewModel();

            main.ShowFocusMode();
            var first = main.CurrentViewModel;
            main.ShowFocusMode();

            Assert.NotSame(first, main.CurrentViewModel);
            await Task.Delay(300);
        }
    }

    [Collection("Database")]
    public class DailyBalanceViewModelTests : DatabaseTestBase
    {
        [Theory]
        [InlineData(1, "Низько (1 з 10)")]
        [InlineData(3, "Низько (3 з 10)")]
        [InlineData(4, "Помірно (4 з 10)")]
        [InlineData(6, "Помірно (6 з 10)")]
        [InlineData(7, "Високо (7 з 10)")]
        [InlineData(8, "Високо (8 з 10)")]
        [InlineData(9, "Дуже високо (9 з 10)")]
        [InlineData(10, "Дуже високо (10 з 10)")]
        public async Task IntensityText_FollowsBoundaries(double value, string expected)
        {
            var vm = new DailyBalanceViewModel(new MainViewModel());

            vm.Intensity = value;

            Assert.Equal(expected, vm.IntensityText);
            await Task.Delay(200);
        }

        [Fact]
        public async Task Emotion_DefaultIsCalm_AndSelectionIsExclusive()
        {
            var vm = new DailyBalanceViewModel(new MainViewModel());
            Assert.True(vm.IsCalm);

            vm.IsTired = true;

            Assert.True(vm.IsTired);
            Assert.False(vm.IsCalm);
            Assert.False(vm.IsAnxious);
            Assert.False(vm.IsIrritated);
            await Task.Delay(200);
        }

        [Fact]
        public async Task SaveMood_StoresEntryTrimsNotesAndClearsField()
        {
            var vm = new DailyBalanceViewModel(new MainViewModel());
            vm.IsAnxious = true;
            vm.Intensity = 7;
            vm.Notes = "  Дедлайн  ";

            vm.SaveMoodCommand.Execute(null);

            var service = new MoodService();
            Assert.True(await WaitUntilAsync(async () => (await service.GetMoodHistoryAsync()).Count == 1));
            var entry = (await service.GetMoodHistoryAsync()).Single();
            Assert.Equal(MoodStates.Anxious, entry.EmotionState);
            Assert.Equal(7, entry.Intensity);
            Assert.Equal("Дедлайн", entry.Notes);
            Assert.True(await WaitUntilAsync(() => vm.StatusMessage == "✓ Збережено: Тривога (7 з 10)"));
            Assert.Equal(string.Empty, vm.Notes);
            Assert.True(await WaitUntilAsync(() => vm.WeekEntriesText == "Збережено записів цього тижня: 1"));
        }

        [Fact]
        public async Task SaveMood_BlankNotes_AreStoredAsNull()
        {
            var vm = new DailyBalanceViewModel(new MainViewModel());
            vm.Notes = "   ";

            vm.SaveMoodCommand.Execute(null);

            var service = new MoodService();
            Assert.True(await WaitUntilAsync(async () => (await service.GetMoodHistoryAsync()).Count == 1));
            Assert.Null((await service.GetMoodHistoryAsync()).Single().Notes);
        }
    }

    [Collection("Database")]
    public class FocusModeViewModelTests : DatabaseTestBase
    {
        [Theory]
        [InlineData(1, "Легка втома (1 з 10)")]
        [InlineData(3, "Легка втома (3 з 10)")]
        [InlineData(4, "Помірна втома (4 з 10)")]
        [InlineData(6, "Помірна втома (6 з 10)")]
        [InlineData(7, "Сильна втома (7 з 10)")]
        [InlineData(8, "Сильна втома (8 з 10)")]
        [InlineData(9, "Виснаження (9 з 10)")]
        [InlineData(10, "Виснаження (10 з 10)")]
        public async Task FatigueText_FollowsBoundaries(double value, string expected)
        {
            var vm = new FocusModeViewModel(new MainViewModel());

            vm.FatigueLevel = value;

            Assert.Equal(expected, vm.FatigueText);
            await Task.Delay(200);
        }

        [Fact]
        public async Task FatigueLevel_IsRoundedToWholeNumber()
        {
            var vm = new FocusModeViewModel(new MainViewModel());

            vm.FatigueLevel = 6.4;

            Assert.Equal(6, vm.FatigueLevel);
            await Task.Delay(200);
        }

        [Fact]
        public async Task LogFatigue_SavesExhaustionEntry_AndShowsItInRecentActivity()
        {
            var vm = new FocusModeViewModel(new MainViewModel());
            vm.FatigueLevel = 8;

            vm.LogFatigueCommand.Execute(null);

            var service = new MoodService();
            Assert.True(await WaitUntilAsync(async () => (await service.GetMoodHistoryAsync()).Count == 1));
            var entry = (await service.GetMoodHistoryAsync()).Single();
            Assert.Equal(MoodStates.Exhaustion, entry.EmotionState);
            Assert.Equal(8, entry.Intensity);

            Assert.True(await WaitUntilAsync(() => vm.Recent.Count == 1));
            Assert.True(await WaitUntilAsync(() => vm.StatusMessage == "✓ Рівень виснаження збережено: 8 з 10"));
            Assert.Equal(System.Windows.Visibility.Collapsed, vm.EmptyVisibility);
        }

        [Fact]
        public async Task RecentActivity_ShowsOnlyLastThreeEntries()
        {
            var service = new MoodService();
            for (int i = 1; i <= 5; i++)
            {
                await service.SaveMoodAsync(MoodStates.Exhaustion, i);
                await Task.Delay(15);
            }

            var vm = new FocusModeViewModel(new MainViewModel());

            Assert.True(await WaitUntilAsync(() => vm.Recent.Count == 3));
            Assert.Contains("(5/10)", vm.Recent[0].Title);
        }

        [Fact]
        public async Task EmptyDatabase_ShowsEmptyHint()
        {
            var vm = new FocusModeViewModel(new MainViewModel());
            await Task.Delay(300);

            Assert.Empty(vm.Recent);
            Assert.Equal(System.Windows.Visibility.Visible, vm.EmptyVisibility);
        }

        [Fact]
        public async Task Habits_AreLoadedFromDatabase_OnOpen()
        {
            await new HabitService().SetAsync("Вода", true);

            var vm = new FocusModeViewModel(new MainViewModel());

            Assert.True(await WaitUntilAsync(() => vm.Habits.Water.IsCompleted));
            Assert.Equal("Виконано сьогодні", vm.Habits.Water.Subtitle);
        }
    }
}
