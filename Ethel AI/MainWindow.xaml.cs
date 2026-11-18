
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
        private TextToSpeechService speechService;
        private AzureSpeechService? azureSpeechService;

        public MainWindow()
        {
            InitializeComponent();
            string? speechKey =
        Environment.GetEnvironmentVariable("ETHEL_AZURE_SPEECH_KEY");

            if (string.IsNullOrWhiteSpace(speechKey))
            {
                MessageBox.Show("Azure Speech key was not found.");
                return;
            }

            azureSpeechService = new AzureSpeechService(speechKey);

            _ = TestAzureVoiceAsync();
        }

        private async Task TestAzureVoiceAsync()
        {
            await SpeakAsync(
                "Hey! I'm Ethel. Ready when you are."
            );
        }

        private async Task SpeakAsync(string text)
        {
            if (azureSpeechService == null)
                return;

            try
            {
                await azureSpeechService.SpeakAsync(text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Speech Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
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
