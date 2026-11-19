
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Diagnostics;

namespace Ethel_AI
{
    public class AIService
    {
        private readonly HttpClient httpClient;


        public AIService()
        {
            httpClient = new HttpClient
            {
                BaseAddress = new Uri("http://localhost:11434/"),
                Timeout = TimeSpan.FromMinutes(5)
            };
        }

        public async Task<string> GetResponseAsync(string message)
        {
            var requestData = new
            {
                model = "qwen3:4b",
                keep_alive = "30m",
                messages = new[]
                {
                    new
                    {
                        role = "system",
                        content = "You are Ethel, a friendly personal AI assistant. " +
                                  "Speak naturally and answer directly. " +
                                  "Keep normal answers between 2 and 4 short sentences. " +
                                  "Avoid unnecessary explanations unless the user asks for detail."
                    },
                    new
                    {
                        role = "user",
                        content = message
                    }
                },
                stream = false,
                think = false,
                options = new
                {
                    num_predict = 100
                }
            };

            string json = JsonSerializer.Serialize(requestData);

            using var content = new StringContent(
                json,
                Encoding.UTF8,
                "application/json"
            );

            using var response = await httpClient.PostAsync(
                "api/chat",
                content
            );

            response.EnsureSuccessStatusCode();

            string responseJson =
                await response.Content.ReadAsStringAsync();

            using JsonDocument document =
    JsonDocument.Parse(responseJson);

            var root = document.RootElement;

            string answer = root
                .GetProperty("message")
                .GetProperty("content")
                .GetString()
                ?? "I'm sorry, I couldn't generate a response.";

            double totalTime =
                root.GetProperty("total_duration").GetDouble() / 1_000_000_000;

            double loadingTime =
                root.GetProperty("load_duration").GetDouble() / 1_000_000_000;

            double promptTime =
                root.GetProperty("prompt_eval_duration").GetDouble() / 1_000_000_000;

            double generationTime =
                root.GetProperty("eval_duration").GetDouble() / 1_000_000_000;

            long generatedTokens =
                root.GetProperty("eval_count").GetInt64();

            double tokensPerSecond =
                generationTime > 0
                    ? generatedTokens / generationTime
                    : 0;

            Debug.WriteLine("===== ETHEL PERFORMANCE =====");
            Debug.WriteLine($"Total time: {totalTime:F2}s");
            Debug.WriteLine($"Model loading: {loadingTime:F2}s");
            Debug.WriteLine($"Prompt processing: {promptTime:F2}s");
            Debug.WriteLine($"Answer generation: {generationTime:F2}s");
            Debug.WriteLine($"Generated tokens: {generatedTokens}");
            Debug.WriteLine($"Tokens per second: {tokensPerSecond:F2}");
            Debug.WriteLine("=============================");

            return answer;
        }
    }
}
