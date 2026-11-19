
using System;
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
            recognitionService = new SpeechRecognitionService(speechKey);

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
            string response;

            string input = message.ToLower().Trim();

            if (input.Contains("hello") || input.Contains("hi"))
            {
                response = "Hello! It's good to hear from you.";
            }
            else if (input.Contains("how are you"))
            {
                response = "I'm doing great! How can I help you?";
            }
            else if (input.Contains("your name"))
            {
                response = "My name is Ethel, your personal AI assistant.";
            }
            else if (input.Contains("thank you"))
            {
                response = "My Pleasure!";
            }
            else
            {
                response = "I heard you, but I don't know how to handle that request yet.";
            }

            AddMessage("Ethel", response);

            await SpeakAsync(response);
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
