using System.Windows;
using DumyWPF.ViewModels;

namespace DumyWPF
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel(); // Обов'язково має бути тут!
        }
    }
}