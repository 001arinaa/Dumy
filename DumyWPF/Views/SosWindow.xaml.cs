using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace DumyWPF.Views
{
    /// <summary>
    /// Вікно SOS-заземлення: «квадратне» дихання 4-4-4-4 з анімацією кола.
    /// </summary>
    public partial class SosWindow : Window
    {
        private const int TotalCycles = 6;
        private const int PhaseSeconds = 4;

        private static readonly (string Name, double Scale)[] Phases =
        {
            ("Вдих", 1.0),
            ("Затримка", 1.0),
            ("Видих", 0.5),
            ("Затримка", 0.5)
        };

        private readonly DispatcherTimer _timer = new() { Interval = TimeSpan.FromSeconds(1) };
        private int _phase;
        private int _second;
        private int _cycle = 1;

        public SosWindow()
        {
            InitializeComponent();

            _timer.Tick += OnTick;
            Loaded += (_, _) => StartPhase();
            Closed += (_, _) => _timer.Stop();
        }

        private void StartPhase()
        {
            var (name, scale) = Phases[_phase];
            _second = PhaseSeconds;

            PhaseText.Text = name;
            CountText.Text = _second.ToString();
            CycleText.Text = "Цикл " + _cycle + " з " + TotalCycles;

            Animate(scale);
            _timer.Start();
        }

        private void OnTick(object? sender, EventArgs e)
        {
            _second--;

            if (_second > 0)
            {
                CountText.Text = _second.ToString();
                return;
            }

            _phase = (_phase + 1) % Phases.Length;
            if (_phase == 0)
            {
                _cycle++;
                if (_cycle > TotalCycles)
                {
                    Finish();
                    return;
                }
            }

            StartPhase();
        }

        private void Animate(double targetScale)
        {
            var duration = new Duration(TimeSpan.FromSeconds(PhaseSeconds));
            var ease = new SineEase { EasingMode = EasingMode.EaseInOut };

            foreach (var scale in new[] { CircleScale, HaloScale })
            {
                double haloFactor = scale == HaloScale ? 1.15 : 1.0;
                var animation = new DoubleAnimation(targetScale * haloFactor, duration) { EasingFunction = ease };
                scale.BeginAnimation(System.Windows.Media.ScaleTransform.ScaleXProperty, animation);
                scale.BeginAnimation(System.Windows.Media.ScaleTransform.ScaleYProperty, animation);
            }
        }

        private void Finish()
        {
            _timer.Stop();
            PhaseText.Text = "Готово";
            CountText.Text = "✓";
            CycleText.Text = "Ви молодець. Як ви почуваєтесь?";
        }

        private void Close_Click(object sender, RoutedEventArgs e) => Close();
    }
}
