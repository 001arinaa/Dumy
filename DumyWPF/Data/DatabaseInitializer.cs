using Microsoft.EntityFrameworkCore;

namespace DumyWPF.Data
{
    public static class DatabaseInitializer
    {
        /// <summary>
        /// Створює dumy.db (з 18 практиками) при першому запуску.
        /// Якщо база вже існує зі старою схемою (без HabitLogs) — дописує відсутню таблицю.
        /// </summary>
        public static void Initialize()
        {
            using var db = new AppDbContext();
            db.Database.EnsureCreated();

            db.Database.ExecuteSqlRaw(
                "CREATE TABLE IF NOT EXISTS \"HabitLogs\" (" +
                "\"Id\" INTEGER NOT NULL CONSTRAINT \"PK_HabitLogs\" PRIMARY KEY AUTOINCREMENT, " +
                "\"Date\" TEXT NOT NULL, " +
                "\"HabitName\" TEXT NOT NULL, " +
                "\"IsCompleted\" INTEGER NOT NULL)");
        }
    }
}
