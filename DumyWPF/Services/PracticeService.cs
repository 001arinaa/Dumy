using DumyWPF.Data;
using DumyWPF.Models;
using Microsoft.EntityFrameworkCore;

namespace DumyWPF.Services
{
    public class PracticeService
    {
        /// <summary>Усі практики.</summary>
        public async Task<List<Practice>> GetAllAsync()
        {
            using var db = new AppDbContext();
            return await db.Practices.AsNoTracking().OrderBy(p => p.Id).ToListAsync();
        }

        /// <summary>Практики профілю: "Щоденний баланс", "Відновлення та фокус" або "Тактичний".</summary>
        public async Task<List<Practice>> GetByProfileTagAsync(string profileTag)
        {
            using var db = new AppDbContext();
            return await db.Practices.AsNoTracking()
                .Where(p => p.ProfileTag == profileTag)
                .OrderBy(p => p.Id).ToListAsync();
        }

        /// <summary>Практики для стану: "Тривога", "Втома", "Дратування" або "Спокій".</summary>
        public async Task<List<Practice>> GetByTargetStateAsync(string targetState)
        {
            using var db = new AppDbContext();
            return await db.Practices.AsNoTracking()
                .Where(p => p.TargetState == targetState)
                .OrderBy(p => p.Id).ToListAsync();
        }

        /// <summary>Практики профілю й стану одночасно (напр. "Тактичний" + "Тривога").</summary>
        public async Task<List<Practice>> GetByProfileAndStateAsync(string profileTag, string targetState)
        {
            using var db = new AppDbContext();
            return await db.Practices.AsNoTracking()
                .Where(p => p.ProfileTag == profileTag && p.TargetState == targetState)
                .OrderBy(p => p.Id).ToListAsync();
        }
    }
}
