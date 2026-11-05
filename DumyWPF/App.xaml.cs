using System.Windows;
using DumyWPF.Data;

namespace DumyWPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            try
            {
                // Створює dumy.db з 18 практиками та таблицю звичок (якщо її ще немає)
                DatabaseInitializer.Initialize();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Не вдалося ініціалізувати базу даних:\n" + ex.Message,
                    "Dumy", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            base.OnStartup(e);
        }
    }
}
