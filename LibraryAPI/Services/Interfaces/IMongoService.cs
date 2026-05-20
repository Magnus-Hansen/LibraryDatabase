using LibraryAPI.DTOs;
using LibraryMongoDBBackend.MongoDesign_Models.Interface;
using LibraryMongoDBBackend.Repositories.Interfaces;

namespace LibraryAPI.Services.Interfaces
{
    public interface IMongoService<TMongo, TDto, CDto, UDto> 
        where TMongo : IMongoModel
        where TDto : class
        where CDto : class
        where UDto : class
    {
        Task<List<TDto>> GetAllAsync();
        Task<TDto> GetByIdAsync(int id);

        Task<TDto> CreateAsync(CDto dto);
        Task<bool> UpdateAsync(UDto dto, int id);
        Task<bool> DeleteAsync(int id);


        TDto MapToDto(TMongo mongoModel);    
    }
}
