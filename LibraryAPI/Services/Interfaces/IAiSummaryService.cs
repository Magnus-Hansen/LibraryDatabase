namespace LibraryAPI.Services.Interfaces
{
    public interface IAiSummaryService
    {
        Task<string> GenerateReviewSummaryAsync(string itemName, IEnumerable<string> reviews);

    }
}
