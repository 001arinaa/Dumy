using DumyWPF.Data;
using DumyWPF.Models;
using DumyWPF.Services;
using DumyWPF.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace DumyWPF.Tests
{
    [Collection("Database")]
    public class SeedDataTests : DatabaseTestBase
    {
        private readonly PracticeService _practices = new();

        [Fact]
        public async Task NewDatabase_Contains18UniquePractices()
        {
            var all = await _practices.GetAllAsync();

            Assert.Equal(18, all.Count);
            Assert.Equal(18, all.Select(p => p.Title).Distinct().Count());
        }

        [Fact]
        public async Task EveryPractice_HasTextAndKnownProfileAndState()
        {
            var profiles = new[] { AppProfiles.Daily, AppProfiles.Focus, AppProfiles.Tactical };
            var states = new[] { MoodStates.Calm, MoodStates.Anxious, MoodStates.Tired, MoodStates.Irritated };

            foreach (var p in await _practices.GetAllAsync())
            {
                Assert.False(string.IsNullOrWhiteSpace(p.Instructions), p.Title);
                Assert.True(p.DurationMin > 0, p.Title);
                Assert.Contains(p.ProfileTag, profiles);
                Assert.Contains(p.TargetState, states);
            }
        }

        [Theory]
        [InlineData(PracticeTitles.Sos)]
        [InlineData(PracticeTitles.TacticalBreathing)]
        [InlineData(PracticeTitles.Grounding)]
        [InlineData(PracticeTitles.EyeExercise)]
        [InlineData(PracticeTitles.Breathing478)]
        public async Task QuickLaunchTitles_ExistInDatabase(string title)
        {
            var practice = await _practices.GetByTitleAsync(title);

            Assert.NotNull(practice);
        }

        [Fact]
        public async Task GetByTitle_UnknownTitle_ReturnsNull()
        {
            Assert.Null(await _practices.GetByTitleAsync("Такої практики немає"));
        }

        [Theory]
        [InlineData(AppProfiles.Daily, 9)]
        [InlineData(AppProfiles.Focus, 6)]
        [InlineData(AppProfiles.Tactical, 3)]
        public async Task GetByProfileTag_ReturnsOnlyThatProfile(string profile, int expectedCount)
        {
            var list = await _practices.GetByProfileTagAsync(profile);

            Assert.Equal(expectedCount, list.Count);
            Assert.All(list, p => Assert.Equal(profile, p.ProfileTag));
        }

        [Fact]
        public async Task GetByProfileAndState_CombinesBothFilters()
        {
            var list = await _practices.GetByProfileAndStateAsync(AppProfiles.Tactical, MoodStates.Anxious);

            Assert.Equal(3, list.Count);
            Assert.All(list, p => Assert.Equal(MoodStates.Anxious, p.TargetState));
        }

        [Fact]
        public void Initialize_CalledTwice_DoesNotDuplicateSeedData()
        {
            DatabaseInitializer.Initialize();
            DatabaseInitializer.Initialize();

            using var db = new AppDbContext();
            Assert.Equal(18, db.Practices.Count());
        }

        [Fact]
        public async Task EyeExercise_HasNoVideoAndTwoMinutes()
        {
            var eyes = await _practices.GetByTitleAsync(PracticeTitles.EyeExercise);

            Assert.NotNull(eyes);
            Assert.Null(eyes!.VideoUrl);
            Assert.Equal(2, eyes.DurationMin);
        }
    }

    [Collection("Database")]
    public class MoodServiceTests : DatabaseTestBase
    {
        private readonly MoodService _service = new();

        [Fact]
        public async Task SaveMood_StoresAllFields()
        {
            await _service.SaveMoodAsync(MoodStates.Tired, 8, "Багато роботи");

            var entry = Assert.Single(await _service.GetMoodHistoryAsync());
            Assert.Equal(MoodStates.Tired, entry.EmotionState);
            Assert.Equal(8, entry.Intensity);
            Assert.Equal("Багато роботи", entry.Notes);
            Assert.True((DateTime.Now - entry.Date).TotalMinutes < 1);
        }

        [Fact]
        public async Task SaveMood_WithoutNotes_StoresNull()
        {
            await _service.SaveMoodAsync(MoodStates.Calm, 3);

            var entry = Assert.Single(await _service.GetMoodHistoryAsync());
            Assert.Null(entry.Notes);
        }

        [Fact]
        public async Task History_IsSortedFromNewestToOldest()
        {
            await _service.SaveMoodAsync(MoodStates.Calm, 1);
            await Task.Delay(20);
            await _service.SaveMoodAsync(MoodStates.Anxious, 2);
            await Task.Delay(20);
            await _service.SaveMoodAsync(MoodStates.Tired, 3);

            var history = await _service.GetMoodHistoryAsync();

            Assert.Equal(new[] { MoodStates.Tired, MoodStates.Anxious, MoodStates.Calm },
                history.Select(h => h.EmotionState).ToArray());
        }

        [Fact]
        public async Task History_EmptyDatabase_ReturnsEmptyList()
        {
            Assert.Empty(await _service.GetMoodHistoryAsync());
        }

        [Fact]
        public async Task CountSince_CountsOnlyRecentEntries()
        {
            await _service.SaveMoodAsync(MoodStates.Calm, 5);
            await _service.SaveMoodAsync(MoodStates.Calm, 5);

            Assert.Equal(2, await _service.CountSinceAsync(DateTime.Now.AddMinutes(-1)));
            Assert.Equal(0, await _service.CountSinceAsync(DateTime.Now.AddMinutes(1)));
        }

        [Fact]
        public async Task ClearHistory_RemovesEverything()
        {
            await _service.SaveMoodAsync(MoodStates.Calm, 5);
            await _service.SaveMoodAsync(MoodStates.Anxious, 6);

            await _service.ClearHistoryAsync();

            Assert.Empty(await _service.GetMoodHistoryAsync());
        }
    }

    [Collection("Database")]
    public class HabitServiceTests : DatabaseTestBase
    {
        private readonly HabitService _service = new();

        private static async Task<int> RowsAsync(string name)
        {
            using var db = new AppDbContext();
            return await db.HabitLogs.CountAsync(h => h.HabitName == name);
        }

        [Fact]
        public async Task GetToday_NoLogs_ReturnsEmptyDictionary()
        {
            Assert.Empty(await _service.GetTodayAsync());
        }

        [Fact]
        public async Task Set_ThenGetToday_ReturnsSavedState()
        {
            await _service.SetAsync("Вода", true);

            var today = await _service.GetTodayAsync();

            Assert.True(today["Вода"]);
        }

        [Fact]
        public async Task Set_SameHabitTwice_UpdatesSingleRow()
        {
            await _service.SetAsync("Сон", true);
            await _service.SetAsync("Сон", false);

            Assert.Equal(1, await RowsAsync("Сон"));
            Assert.False((await _service.GetTodayAsync())["Сон"]);
        }

        [Fact]
        public async Task Set_DifferentHabits_AreStoredSeparately()
        {
            await _service.SetAsync("Вода", true);
            await _service.SetAsync("Фокус", false);

            var today = await _service.GetTodayAsync();

            Assert.True(today["Вода"]);
            Assert.False(today["Фокус"]);
            Assert.False(today.ContainsKey("Сон"));
        }

        [Fact]
        public async Task GetToday_IgnoresYesterdayLogs()
        {
            using (var db = new AppDbContext())
            {
                db.HabitLogs.Add(new HabitLog { Date = DateTime.Today.AddDays(-1), HabitName = "Вода", IsCompleted = true });
                await db.SaveChangesAsync();
            }

            Assert.Empty(await _service.GetTodayAsync());
        }

        [Fact]
        public async Task Set_NewDay_CreatesNewRowAndKeepsHistory()
        {
            using (var db = new AppDbContext())
            {
                db.HabitLogs.Add(new HabitLog { Date = DateTime.Today.AddDays(-1), HabitName = "Вода", IsCompleted = true });
                await db.SaveChangesAsync();
            }

            await _service.SetAsync("Вода", false);

            Assert.Equal(2, await RowsAsync("Вода"));
            Assert.False((await _service.GetTodayAsync())["Вода"]);
        }

        [Fact]
        public async Task Set_ManyQuickToggles_NeverCreatesDuplicates()
        {
            var tasks = Enumerable.Range(0, 20).Select(i => _service.SetAsync("Вода", i % 2 == 0));

            await Task.WhenAll(tasks);

            Assert.Equal(1, await RowsAsync("Вода"));
        }

        [Fact]
        public async Task ClearAll_RemovesAllLogs()
        {
            await _service.SetAsync("Вода", true);
            await _service.SetAsync("Сон", true);

            await _service.ClearAllAsync();

            Assert.Empty(await _service.GetTodayAsync());
        }
    }
}
