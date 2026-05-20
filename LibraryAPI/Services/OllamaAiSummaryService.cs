using LibraryAPI.Services.Interfaces;
using System.Text.Json;

namespace LibraryAPI.Services
{
    public class OllamaAiSummaryService : IAiSummaryService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public OllamaAiSummaryService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<string> GenerateReviewSummaryAsync(string itemName, IEnumerable<string> reviews)
        {
            var model = _configuration["Ai:Model"] ?? "llama3.2:latest";

            var reviewList = reviews.ToList();

            var reviewText = string.Join("\n", reviewList.Select((review, index) => $"{index + 1}. {review}"));

            var prompt = $"""
                    You are summarizing user reviews for a library system.

                    IMPORTANT RULES:
                    - Only use information explicitly written in the reviews.
                    - Do not use outside knowledge about the item, author, franchise, genre, characters, or story.
                    - Do not mention details that are not present in the reviews.
                    - If there is only one short review, write one short sentence.
                    - Do not say "reviewers" if there is only one review.
                    - Do not say "commonly", "generally", "widely", or "overall" unless multiple reviews support it.
                    - Keep the summary factual and modest.
                    - Maximum length: 100 words.

                    Item title:
                    {itemName}

                    Number of reviews:
                    {reviewList.Count}

                    Reviews:
                    {reviewText}

                    Write only the review summary.
                    """;

            var request = new
            {
                model,
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

            var response = await _httpClient.PostAsJsonAsync("/api/chat", request);

            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();

            using var json = JsonDocument.Parse(responseContent);

            var summary = json.RootElement
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

            return summary?.Trim() ?? string.Empty;
        }
    }
}
