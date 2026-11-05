using DumyWPF.Data;
using DumyWPF.Models;
using Microsoft.EntityFrameworkCore;

namespace DumyWPF.Services
{
    public class MoodService
    {
        /// <summary>Зберігає настрій. Дата ставиться автоматично.</summary>
        public async Task SaveMoodAsync(string emotionState, int intensity, string? notes = null)
        {
            using var db = new AppDbContext();
            db.MoodEntries.Add(new MoodEntry
            {
                Date = DateTime.Now,
                EmotionState = emotionState,
                Intensity = intensity,
                Notes = notes
            });
            await db.SaveChangesAsync();
        }

        /// <summary>Історія настрою: від найновіших до найстаріших.</summary>
        public async Task<List<MoodEntry>> GetMoodHistoryAsync()
        {
            using var db = new AppDbContext();
            return await db.MoodEntries.AsNoTracking()
                .OrderByDescending(m => m.Date).ToListAsync();
        }

        /// <summary>Кількість записів, зроблених починаючи з вказаної дати.</summary>
        public async Task<int> CountSinceAsync(DateTime since)
        {
            using var db = new AppDbContext();
            return await db.MoodEntries.CountAsync(m => m.Date >= since);
        }

        /// <summary>Видаляє всю історію настрою.</summary>
        public async Task ClearHistoryAsync()
        {
            using var db = new AppDbContext();
            await db.MoodEntries.ExecuteDeleteAsync();
        }
    }
}
