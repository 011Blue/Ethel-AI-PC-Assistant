
using Microsoft.CognitiveServices.Speech;
using System.Windows;

namespace Ethel_AI
{
    public class AzureSpeechService
    {
        private readonly SpeechConfig speechConfig;

        public AzureSpeechService(string subscriptionKey)
        {
            speechConfig = SpeechConfig.FromSubscription(
                subscriptionKey,
                "southafricanorth"
            );

            // Ethel's initial neural voice
            speechConfig.SpeechSynthesisVoiceName =
                "en-US-JennyNeural";
        }


        public async Task SpeakAsync(string text)
        {
            using var synthesizer = new SpeechSynthesizer(speechConfig);

            using var result = await synthesizer.SpeakTextAsync(text);

            if (result.Reason == ResultReason.SynthesizingAudioCompleted)
            {
                Console.WriteLine(
                    $"Audio generated successfully. Characters: {text.Length}"
                );
            }
            else
            {
                throw new Exception(
                    $"Speech synthesis failed: {result.Reason}"
                );
            }
        }

    }
}
