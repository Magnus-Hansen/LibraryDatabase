using LibraryAPI.DTOs;

namespace LibraryAPI.Services.Graph.Interfaces
{
    public interface ILoanerServiceGraph
    {
        Task<IEnumerable<LoanerDto>> GetAllAsync();
        Task<LoanerDto?> GetByIdAsync(int id);
        Task<LoanerDto> RegisterAsync(RegisterLoanerDto dto);
        Task<AuthResponseDto> LoginAsync(LoginDto dto);
    }
}
