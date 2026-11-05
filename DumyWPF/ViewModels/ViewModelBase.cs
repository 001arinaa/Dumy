using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace DumyWPF.ViewModels
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>Виконує асинхронну дію й показує зрозуміле повідомлення замість падіння застосунку.</summary>
        protected static async Task RunSafeAsync(Func<Task> action)
        {
            try
            {
                await action();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Сталася помилка:\n" + ex.Message, "Dumy",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
