using LibraryAPI.DTOs;

namespace LibraryAPI.Services.Graph.Interfaces
{
    public interface IItemServiceGraph
    {
        Task<PagedResultDto<ItemDto>> GetAllAsync(int page, int pageSize);
        Task<PagedResultDto<ItemDto>> GetItemsByGenreAsync(int genreId, int page, int pageSize);
        Task<ItemDetailsDto?> GetByIdAsync(int id);
        Task<ItemDetailsDto> AddAsync(CreateItemDto itemDto);
        Task<bool> UpdateAsync(int id, UpdateItemDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
