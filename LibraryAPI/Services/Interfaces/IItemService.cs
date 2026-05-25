using LibraryAPI.DTOs;
using LibrarySQLBackend.Models;

namespace LibraryAPI.Services.Interfaces
{
    public interface IItemService
    {
        Task<ItemDetailsDto?> GetByIdAsync(int id);
        Task<ItemDetailsDto> AddAsync(CreateItemDto itemDto);
        Task<bool> UpdateAsync(int id, UpdateItemDto dto);
        Task<(bool Success, string Message)> DeleteAsync(int id);
        Task<ReviewSummaryResultDto?> GenerateReviewSummaryAsync(int id);

        Task<PagedResultDto<ItemDto>> GetAllAsync(int pageNumber);
        Task<PagedResultDto<ItemDto>> GetByMediaTypeAsync(string mediaType, int pageNumber);
    }
}
