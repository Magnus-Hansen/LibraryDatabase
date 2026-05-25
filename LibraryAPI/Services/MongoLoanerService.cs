using LibraryAPI.DTOs;
using LibraryAPI.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using MongoDB.Bson;

namespace LibraryAPI.Services
{
    public class MongoLoanerService : IMongoService<LoanersMongo, LoanerDto, RegisterLoanerDto, LoanerDto, LoanerDto>
    {
        private readonly MongoRepository<LoanersMongo> _repository;
        private readonly IPasswordHasher<LoanersMongo> _passwordHasher;

        public MongoLoanerService(MongoDbContext context)
        {
            _repository = new MongoRepository<LoanersMongo>(context, "Loaners");
            _passwordHasher = new PasswordHasher<LoanersMongo>();
        }
        public async Task<LoanerDto> CreateAsync(RegisterLoanerDto dto)
        {
            var loaner = new LoanersMongo()
            {
                Id = (await GetAllAsync()).Count + 1,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Cpr = dto.Cpr,
                Tlf = dto.Tlf,
                Email = dto.Email
            };
            loaner.PasswordHash = _passwordHasher.HashPassword(loaner, dto.Password!);

            await _repository.CreateAsync(loaner);

            return MapToDto(loaner);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                return await _repository.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                throw new Exception($"Error deleting loaner with ID {id}: {ex.Message}", ex);
            }
        }

        public async Task<List<LoanerDto>> GetAllAsync()
        {
            var loaners = await _repository.GetAllAsync();
            return loaners.Select(MapToDto).ToList();
        }

        public async Task<LoanerDto> GetByIdAsync(int id)
        {
            var loaner = await _repository.GetByIdAsync(id);
            if (loaner == null)
                return null;
            return MapToDto(loaner);
        }

        private async Task<string> GetMongoId(int id)
        {
            LoanersMongo loan = await _repository.GetByIdAsync(id);
            return loan?._id.ToString();
        }
        public async Task<bool> UpdateAsync(LoanerDto dto, int id)
        {
            if (await GetByIdAsync(id) != null)
            {
                await _repository.UpdateAsync(id, new LoanersMongo
                {
                    _id = ObjectId.Parse(await GetMongoId(id)),
                    Id = id,
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    Cpr = dto.Cpr,
                    Tlf = dto.Tlf,
                    Email = dto.Email
                });
                return true;
            }
            return false;
        }

        public LoanerDto MapToDto(LoanersMongo mongoModel)
        {
            return new LoanerDto
            {
                Id = mongoModel.Id,
                FirstName = mongoModel.FirstName,
                LastName = mongoModel.LastName,
                Cpr = mongoModel.Cpr,
                Tlf = mongoModel.Tlf,
                Email = mongoModel.Email
            };
        }
    }
}
