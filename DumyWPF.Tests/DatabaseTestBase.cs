using System.IO;
using DumyWPF.Data;
using Microsoft.Data.Sqlite;

namespace DumyWPF.Tests
{
    /// <summary>Усі тести, що працюють з БД, виконуються по черзі (спільний шлях до файлу бази).</summary>
    [CollectionDefinition("Database", DisableParallelization = true)]
    public class DatabaseCollection { }

    /// <summary>
    /// Кожен тест отримує власну порожню базу у тимчасовій папці
    /// (з 18 практиками) і ніколи не чіпає справжній dumy.db.
    /// </summary>
    public abstract class DatabaseTestBase : IDisposable
    {
        private readonly string _dbFile;

        protected DatabaseTestBase()
        {
            _dbFile = Path.Combine(Path.GetTempPath(), "dumy-test-" + Guid.NewGuid().ToString("N") + ".db");
            AppDbContext.DbPathOverride = _dbFile;
            DatabaseInitializer.Initialize();
        }

        public void Dispose()
        {
            // Шлях навмисно не скидаємо на справжню БД: пізні фонові збереження не мають її зачепити.
            SqliteConnection.ClearAllPools();
            try { File.Delete(_dbFile); } catch { /* файл може бути ще зайнятий — це не помилка тесту */ }
        }

        /// <summary>Чекає, поки умова стане true (для фонових збережень), максимум timeoutMs.</summary>
        protected static async Task<bool> WaitUntilAsync(Func<Task<bool>> condition, int timeoutMs = 5000)
        {
            var until = DateTime.UtcNow.AddMilliseconds(timeoutMs);
            while (DateTime.UtcNow < until)
            {
                if (await condition()) return true;
                await Task.Delay(25);
            }
            return await condition();
        }

        protected static Task<bool> WaitUntilAsync(Func<bool> condition, int timeoutMs = 5000)
            => WaitUntilAsync(() => Task.FromResult(condition()), timeoutMs);
    }
}
