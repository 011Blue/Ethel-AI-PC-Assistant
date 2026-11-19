
using System;
using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
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
        private SpeechRecognitionService? recognitionService;
        private AIService? aiService;

        public MainWindow()
        {
            InitializeComponent();
            aiService = new AIService();
            string? speechKey = Environment.GetEnvironmentVariable("ETHEL_AZURE_SPEECH_KEY");

            if (string.IsNullOrWhiteSpace(speechKey))
            {
                MessageBox.Show("Azure Speech key was not found.");
                return;
            }

            azureSpeechService = new AzureSpeechService(speechKey);
            recognitionService = new SpeechRecognitionService(speechKey);

            _ = TestAzureVoiceAsync();
        }

        private async Task TestAzureVoiceAsync()
        {
            await SpeakAsync(
                "Hey! I'm Ethel. Your Personal Assistant."
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

        private void AddMessage(string sender, string message)
        {
            bool isUser = sender == "You";

            Border messageBorder = new Border
            {
                Background = new SolidColorBrush(
                    (Color)ColorConverter.ConvertFromString(
                        isUser ? "#374151" : "#312E81")
                ),

                CornerRadius = new CornerRadius(12),
                Padding = new Thickness(12),
                Margin = new Thickness(0, 0, 0, 12),
                MaxWidth = 350,
                HorizontalAlignment = isUser
                    ? HorizontalAlignment.Right
                    : HorizontalAlignment.Left
            };

            StackPanel messageContent = new StackPanel();

            TextBlock senderText = new TextBlock
            {
                Text = sender.ToUpper(),
                Foreground = new SolidColorBrush(
                    (Color)ColorConverter.ConvertFromString("#C4B5FD")
                ),
                FontWeight = FontWeights.Bold,
                FontSize = 11,
                Margin = new Thickness(0, 0, 0, 5)
            };

            TextBlock messageText = new TextBlock
            {
                Text = message,
                Foreground = Brushes.White,
                FontSize = 14,
                TextWrapping = TextWrapping.Wrap
            };

            messageContent.Children.Add(senderText);
            messageContent.Children.Add(messageText);

            messageBorder.Child = messageContent;

            ConversationPanel.Children.Add(messageBorder);
        }


        private async Task ProcessMessageAsync(string message)
        {
            var timer = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                StatusText.Text = "● Thinking...";

                string response = await aiService!.GetResponseAsync(message);

                timer.Stop();

                AddMessage("Ethel", response);

                StatusText.Text = $"● AI replied in {timer.Elapsed.TotalSeconds:F1}s";

                var speechTimer = System.Diagnostics.Stopwatch.StartNew();

                await SpeakAsync(response);

                speechTimer.Stop();

                StatusText.Text =
                    $"● Ready | AI: {timer.Elapsed.TotalSeconds:F1}s | Voice: {speechTimer.Elapsed.TotalSeconds:F1}s";
            }
            catch (Exception ex)
            {
                StatusText.Text = "● Error";

                MessageBox.Show(
                    ex.Message,
                    "Ethel Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }


        private async void MicrophoneButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                StatusText.Text = "● Listening...";
                StatusText.Foreground = Brushes.MediumPurple;

                MicrophoneButton.IsEnabled = false;

                string spokenText =
                    await recognitionService!.RecognizeSpeechAsync();

                if (string.IsNullOrWhiteSpace(spokenText))
                {
                    StatusText.Text = "● Nothing heard";
                    return;
                }

                StatusText.Text = "● Speech recognised";

                AddMessage("You", spokenText);
                await ProcessMessageAsync(spokenText);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Speech Recognition Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );

                StatusText.Text = "● Error";
            }
            finally
            {
                MicrophoneButton.IsEnabled = true;
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
