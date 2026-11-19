
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;

namespace Ethel_AI
{
    public class SpeechRecognitionService
    {
        private readonly SpeechConfig speechConfig;

        public SpeechRecognitionService(string subscriptionKey)
        {
            speechConfig = SpeechConfig.FromSubscription(
                subscriptionKey,
                "southafricanorth"
            );

            // South African English recognition
            speechConfig.SpeechRecognitionLanguage = "en-ZA";
        }

        public async Task<string> RecognizeSpeechAsync()
        {
            using var audioConfig =
                AudioConfig.FromDefaultMicrophoneInput();

            using var recognizer =
                new SpeechRecognizer(speechConfig, audioConfig);

            var result = await recognizer.RecognizeOnceAsync();

            switch (result.Reason)
            {
                case ResultReason.RecognizedSpeech:
                    return result.Text;

                case ResultReason.NoMatch:
                    return "";

                case ResultReason.Canceled:
                    var cancellation =
                        CancellationDetails.FromResult(result);

                    throw new Exception(
                        $"Speech recognition cancelled: " +
                        $"{cancellation.Reason}. " +
                        $"{cancellation.ErrorDetails}"
                    );

                default:
                    return "";
            }
        }
    }
}
