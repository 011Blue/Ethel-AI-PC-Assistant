
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
                BaseAddress = new Uri("http://localhost:11434/")
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
                        content = "You are Ethel, a friendly and helpful personal AI assistant for Windows."
                    },
                    new
                    {
                        role = "user",
                        content = message
                    }
                },
                stream = false
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

            string responseJson = await response.Content.ReadAsStringAsync();

            using JsonDocument document = JsonDocument.Parse(responseJson);

            return document.RootElement
                .GetProperty("message")
                .GetProperty("content")
                .GetString() ?? "I'm sorry, I couldn't generate a response.";
        }
    }
}
