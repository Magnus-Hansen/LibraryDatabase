using LibraryAPI.DTOs;

namespace LibraryAPI.Services.Graph.Interfaces
{
    public interface IItemServiceGraph
    {
        Task<IEnumerable<ItemDto>> GetAllAsync();
        Task<ItemDetailsDto?> GetByIdAsync(int id);
        Task<ItemDetailsDto> AddAsync(CreateItemDto itemDto);
        Task<bool> UpdateAsync(int id, UpdateItemDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
