using System.Windows;
using DumyWPF.Data;
using Microsoft.EntityFrameworkCore;

namespace DumyWPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            // Створює dumy.db і застосовує міграції (разом із 18 практиками) при першому запуску
            using (var db = new AppDbContext())
            {
                db.Database.EnsureCreated();
            }

            base.OnStartup(e);
        }
    }
}
