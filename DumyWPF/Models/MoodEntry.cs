namespace DumyWPF.Models
{
    /// <summary>Запис настрою користувача в історії.</summary>
    public class MoodEntry
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string EmotionState { get; set; } = string.Empty;
        public int Intensity { get; set; }
        public string? Notes { get; set; }
    }
}
