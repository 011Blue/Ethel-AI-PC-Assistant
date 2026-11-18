
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Ethel_AI
{
    public partial class MainWindow : Window
    {
        private bool isListening = false;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void MicrophoneButton_Click(object sender, RoutedEventArgs e)
        {
            isListening = !isListening;

            if (isListening)
            {
                StatusText.Text = "● Listening...";
                StatusText.Foreground = Brushes.MediumPurple;

                MicrophoneButton.Content = "⏹ Stop";

                StartWaveAnimation();
            }
            else
            {
                StatusText.Text = "● Idle";
                StatusText.Foreground = Brushes.MediumSeaGreen;

                MicrophoneButton.Content = "🎙";

                StopWaveAnimation();
            }
        }

        private void StartWaveAnimation()
        {
            Rectangle[] waves =
            {
                Wave1, Wave2, Wave3, Wave4,
                Wave5, Wave6, Wave7
            };

            for (int i = 0; i < waves.Length; i++)
            {
                DoubleAnimation animation = new DoubleAnimation
                {
                    From = 0.2,
                    To = 1.0,
                    Duration = TimeSpan.FromSeconds(0.35),
                    AutoReverse = true,
                    RepeatBehavior = RepeatBehavior.Forever,
                    BeginTime = TimeSpan.FromSeconds(i * 0.08),

                    EasingFunction = new SineEase
                    {
                        EasingMode = EasingMode.EaseInOut
                    }
                };

                ScaleTransform transform =
                    (ScaleTransform)waves[i].RenderTransform;

                transform.BeginAnimation(
                    ScaleTransform.ScaleYProperty,
                    animation);
            }
        }

        private void StopWaveAnimation()
        {
            Rectangle[] waves =
            {
                Wave1, Wave2, Wave3, Wave4,
                Wave5, Wave6, Wave7
            };

            foreach (Rectangle wave in waves)
            {
                ScaleTransform transform =
                    (ScaleTransform)wave.RenderTransform;

                transform.BeginAnimation(
                    ScaleTransform.ScaleYProperty, null);

                transform.ScaleY = 0.2;
            }
        }
    }
}
