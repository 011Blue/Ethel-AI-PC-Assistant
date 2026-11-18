
using System.Speech.Synthesis;

namespace Ethel_AI
{
    public class VoiceSettingsService
    {
        private readonly SpeechSynthesizer synthesizer;

        public VoiceSettingsService()
        {
            synthesizer = new SpeechSynthesizer();

            synthesizer.Volume = 100;
            synthesizer.Rate = 0;
        }

        // Get all voices installed on Windows

        public List<string> GetAvailableVoices()
        {
            List<string> voices = new List<string>();

            foreach (InstalledVoice voice in synthesizer.GetInstalledVoices())
            {
                if (voice.Enabled)
                {
                    string name = voice.VoiceInfo.Name;
                    string gender = voice.VoiceInfo.Gender.ToString();

                    voices.Add($"{name} ({gender})");
                }
            }

            return voices;
        }

        public List<string> GetFemaleVoices()
        {
            List<string> voices = new List<string>();

            foreach (InstalledVoice voice in synthesizer.GetInstalledVoices())
            {
                if (voice.Enabled &&
                    voice.VoiceInfo.Gender == VoiceGender.Female)
                {
                    voices.Add(voice.VoiceInfo.Name);
                }
            }

            return voices;
        }




        // Select Ethel's voice
        public void SelectVoice(string voiceName)
        {
            synthesizer.SelectVoice(voiceName);
        }

        // Change speaking speed
        public void SetRate(int rate)
        {
            synthesizer.Rate = rate;
        }

        // Change volume
        public void SetVolume(int volume)
        {
            synthesizer.Volume = volume;
        }

        // Preview or speak text
        public void Speak(string text)
        {
            synthesizer.SpeakAsync(text);
        }

        // Stop speaking
        public void StopSpeaking()
        {
            synthesizer.SpeakAsyncCancelAll();
        }
    }
}
