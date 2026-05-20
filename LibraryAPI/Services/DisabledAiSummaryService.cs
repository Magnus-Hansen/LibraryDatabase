using LibraryAPI.Services.Interfaces;

namespace LibraryAPI.Services
{
    public class DisabledAiSummaryService : IAiSummaryService
    {
        public Task<string> GenerateReviewSummaryAsync(string itemName, IEnumerable<string> reviews)
        {
            throw new InvalidOperationException("AI summary generation is disabled in this environment.");
        }
    }
}
