using LibraryAPI.DTOs;

namespace LibraryAPI.Services.Interfaces
{
    public interface IMongoService
    {
        Task GetAllAsync();
        Task GetOneAsync();
        Task Create();
        Task Update();
        Task Delete();

        Task MapToDto();    
    }
}
