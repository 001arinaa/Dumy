namespace DumyWPF.Models
{
    /// <summary>Щоденна відмітка звички (Вода / Сон / Фокус).</summary>
    public class HabitLog
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string HabitName { get; set; } = string.Empty;
        public bool IsCompleted { get; set; }
    }
}
