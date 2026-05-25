using LibraryAPI.DTOs;

namespace LibraryAPI.Services.Graph.Interfaces
{
    public interface IItemServiceGraph
    {
        Task<PagedResultDto<ItemDto>> GetAllAsync(int page);
        Task<PagedResultDto<ItemDto>> GetByMediatypeAsync(string mediatype, int page);
        Task<ItemDetailsDto?> GetByIdAsync(int id);
        Task<ItemDetailsDto> AddAsync(CreateItemDto itemDto);
        Task<bool> UpdateAsync(int id, UpdateItemDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
