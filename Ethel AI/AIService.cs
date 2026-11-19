
using System.Net.Http;
using System.Text;
using System.Text.Json;

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

            return document.RootElement
                .GetProperty("message")
                .GetProperty("content")
                .GetString()
                ?? "I'm sorry, I couldn't generate a response.";
        }
    }
}
