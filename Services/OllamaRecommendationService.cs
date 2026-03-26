using System.Text;
using System.Text.Json;
using Shortlist.Web.Models;

namespace Shortlist.Web.Services
{
    public class OllamaRecommendationService
    {
        private readonly HttpClient _httpClient;

        public OllamaRecommendationService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<AiBestAreaResponse> GenerateBestAreaSummaryAsync(
            string areaName,
            int score,
            List<string> priorities,
            int matchCount,
            double avgDistanceKm)
        {
            var prioritiesText = priorities != null && priorities.Count > 0
                ? string.Join(", ", priorities)
                : "general housing preferences";

            var prompt = $"""
You are helping a rental housing platform called Shortlist.

Write a short premium recommendation for the best area.
Keep it professional, clear, and easy to understand.
Maximum 3 sentences.
Do not use markdown.
Do not use bullet points.

Area name: {areaName}
Recommendation score: {score}/100
User priorities: {prioritiesText}
Nearby matches: {matchCount}
Average distance: {avgDistanceKm:F2} km

Return only the explanation text.
""";

            var requestBody = new
            {
                model = "llama3.2:3b",
                messages = new[]
                {
                    new
                    {
                        role = "user",
                        content = prompt
                    }
                },
                stream = false
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("http://localhost:11434/api/chat", content);
            response.EnsureSuccessStatusCode();

            var raw = await response.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(raw);

            var summary =
                doc.RootElement
                   .GetProperty("message")
                   .GetProperty("content")
                   .GetString() ?? "No AI summary available.";

            return new AiBestAreaResponse
            {
                AreaName = areaName,
                Score = score,
                Summary = summary.Trim()
            };
        }
    }
}