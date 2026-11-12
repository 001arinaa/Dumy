using DumyWPF.Data;
using DumyWPF.Models;
using Microsoft.EntityFrameworkCore;

namespace DumyWPF.Services
{
    public class HabitService
    {
        // Один запис у БД за раз: швидкі кліки не створюють дублів і не змагаються за файл.
        private static readonly SemaphoreSlim Gate = new(1, 1);

        /// <summary>Стан звичок за сьогодні: назва звички -> виконано чи ні.</summary>
        public async Task<Dictionary<string, bool>> GetTodayAsync()
        {
            using var db = new AppDbContext();
            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);

            var logs = await db.HabitLogs.AsNoTracking()
                .Where(h => h.Date >= today && h.Date < tomorrow)
                .ToListAsync();

            return logs
                .GroupBy(h => h.HabitName)
                .ToDictionary(g => g.Key, g => g.OrderBy(h => h.Id).Last().IsCompleted);
        }

        /// <summary>Зберігає відмітку звички за сьогодні (оновлює запис, якщо він уже є).</summary>
        public async Task SetAsync(string habitName, bool isCompleted)
        {
            await Gate.WaitAsync();
            try
            {
            using var db = new AppDbContext();
            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);

            var log = await db.HabitLogs.FirstOrDefaultAsync(h =>
                h.HabitName == habitName && h.Date >= today && h.Date < tomorrow);

            if (log == null)
            {
                db.HabitLogs.Add(new HabitLog
                {
                    Date = DateTime.Now,
                    HabitName = habitName,
                    IsCompleted = isCompleted
                });
            }
            else
            {
                log.IsCompleted = isCompleted;
                log.Date = DateTime.Now;
            }

            await db.SaveChangesAsync();
            }
            finally
            {
                Gate.Release();
            }
        }

        /// <summary>Видаляє всі відмітки звичок.</summary>
        public async Task ClearAllAsync()
        {
            using var db = new AppDbContext();
            await db.HabitLogs.ExecuteDeleteAsync();
        }
    }
}
