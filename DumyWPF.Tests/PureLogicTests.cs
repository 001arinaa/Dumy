using System.Windows.Media;
using DumyWPF.Models;
using DumyWPF.ViewModels;

namespace DumyWPF.Tests
{
    /// <summary>Тести без бази даних: прості правила та форматування.</summary>
    public class MoodStatesTests
    {
        [Theory]
        [InlineData(MoodStates.Calm, "😊")]
        [InlineData(MoodStates.Anxious, "😟")]
        [InlineData(MoodStates.Tired, "😫")]
        [InlineData(MoodStates.Irritated, "😡")]
        [InlineData(MoodStates.Sos, "🚨")]
        [InlineData(MoodStates.TacticalState, "⚡")]
        [InlineData(MoodStates.Exhaustion, "⚙")]
        public void EmojiFor_KnownState_ReturnsItsEmoji(string state, string expected)
        {
            Assert.Equal(expected, MoodStates.EmojiFor(state));
        }

        [Theory]
        [InlineData("")]
        [InlineData("Невідомий стан")]
        public void EmojiFor_UnknownState_ReturnsDefaultNote(string state)
        {
            Assert.Equal("📝", MoodStates.EmojiFor(state));
        }
    }

    public class RelayCommandTests
    {
        [Fact]
        public void Execute_CallsActionWithParameter()
        {
            object? received = null;
            var cmd = new RelayCommand(p => received = p);

            cmd.Execute("hello");

            Assert.Equal("hello", received);
        }

        [Fact]
        public void CanExecute_WithoutPredicate_IsAlwaysTrue()
        {
            var cmd = new RelayCommand(_ => { });

            Assert.True(cmd.CanExecute(null));
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void CanExecute_UsesPredicate(bool allowed)
        {
            var cmd = new RelayCommand(_ => { }, _ => allowed);

            Assert.Equal(allowed, cmd.CanExecute(null));
        }

        [Fact]
        public void Constructor_NullAction_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new RelayCommand(null!));
        }
    }

    public class HistoryItemViewModelTests
    {
        private static MoodEntry Entry(string state, int intensity = 5, string? notes = null, DateTime? date = null) =>
            new() { EmotionState = state, Intensity = intensity, Notes = notes, Date = date ?? DateTime.Now };

        [Fact]
        public void Mood_ShowsStateAndIntensity()
        {
            var vm = new HistoryItemViewModel(Entry(MoodStates.Anxious, 7));

            Assert.Equal("Настрій: Тривога (7/10)", vm.Title);
            Assert.Equal("😟 Настрій: Тривога (7/10)", vm.Summary);
        }

        [Fact]
        public void Sos_HasFixedTitle()
        {
            var vm = new HistoryItemViewModel(Entry(MoodStates.Sos, 0));

            Assert.Equal("SOS Заземлення", vm.Title);
        }

        [Fact]
        public void TacticalState_ShowsLevel()
        {
            var vm = new HistoryItemViewModel(Entry(MoodStates.TacticalState, 4));

            Assert.Equal("Тактична фіксація стресу (4/10)", vm.Title);
        }

        [Fact]
        public void Exhaustion_ShowsLevel()
        {
            var vm = new HistoryItemViewModel(Entry(MoodStates.Exhaustion, 9));

            Assert.Equal("Рівень виснаження (9/10)", vm.Title);
        }

        [Fact]
        public void Notes_AddMarkerToTitle()
        {
            var vm = new HistoryItemViewModel(Entry(MoodStates.Calm, 5, "Гарний день"));

            Assert.Equal("Гарний день", vm.Notes);
            Assert.EndsWith(" 📝", vm.Title);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void EmptyNotes_AreTreatedAsNoNotes(string? notes)
        {
            var vm = new HistoryItemViewModel(Entry(MoodStates.Calm, 5, notes));

            Assert.Null(vm.Notes);
            Assert.DoesNotContain("📝", vm.Title);
        }

        [Theory]
        [InlineData(MoodStates.Sos, 0xD9, 0x53, 0x4F)]
        [InlineData(MoodStates.TacticalState, 0xA3, 0xA0, 0x75)]
        [InlineData(MoodStates.Exhaustion, 0x9A, 0xA3, 0xB6)]
        [InlineData(MoodStates.Calm, 0xDA, 0xB6, 0xC8)]
        public void DotBrush_MatchesProfileColor(string state, int r, int g, int b)
        {
            var vm = new HistoryItemViewModel(Entry(state));

            var brush = Assert.IsType<SolidColorBrush>(vm.DotBrush);
            Assert.Equal(Color.FromRgb((byte)r, (byte)g, (byte)b), brush.Color);
            Assert.True(brush.IsFrozen);
        }

        [Fact]
        public void DateText_Today_StartsWithToday()
        {
            var vm = new HistoryItemViewModel(Entry(MoodStates.Calm, date: DateTime.Now));

            Assert.StartsWith("Сьогодні, ", vm.DateText);
        }

        [Fact]
        public void DateText_Yesterday_StartsWithYesterday()
        {
            var vm = new HistoryItemViewModel(Entry(MoodStates.Calm, date: DateTime.Now.AddDays(-1)));

            Assert.StartsWith("Вчора, ", vm.DateText);
        }

        [Fact]
        public void DateText_OlderDate_UsesUkrainianMonth()
        {
            var vm = new HistoryItemViewModel(Entry(MoodStates.Calm, date: new DateTime(2024, 3, 5, 10, 0, 0)));

            Assert.Equal("5 березня", vm.DateText);
        }
    }
}
