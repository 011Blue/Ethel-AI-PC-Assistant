
using System.Speech.Synthesis;

namespace Ethel_AI
{
    public class TextToSpeechService
    {
        private SpeechSynthesizer synthesizer;

        public TextToSpeechService()
        {
            synthesizer = new SpeechSynthesizer();
            synthesizer.Volume = 100;
            synthesizer.Rate = 0;
        }

        public void Speak(string text)
        {
            synthesizer.SpeakAsync(text);
        }

        public void StopSpeaking()
        {
            synthesizer.SpeakAsyncCancelAll();
        }

        public void ListAvailableVoices()
        {
            foreach (InstalledVoice voice in synthesizer.GetInstalledVoices())
            {
                Console.WriteLine(voice.VoiceInfo.Name);
            }
        }
    }
}
