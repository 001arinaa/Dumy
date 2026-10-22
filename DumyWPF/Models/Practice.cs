namespace DumyWPF.Models
{
    /// <summary>
    /// Практика зі стартової бібліотеки (18 записів). Назви полів збігаються з UML-схемою
    /// і з колонками таблиці контенту — саме їх використовуємо в DataBinding у XAML.
    /// </summary>
    public class Practice
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string ProfileTag { get; set; } = string.Empty;
        public string? TargetState { get; set; }
        public int DurationMin { get; set; }
        public string Instructions { get; set; } = string.Empty;
        public string? VideoUrl { get; set; }
    }
}
