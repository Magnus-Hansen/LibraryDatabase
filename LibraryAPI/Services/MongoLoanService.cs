using LibraryAPI.DTOs;
using LibraryAPI.Services.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using MongoDB.Bson;
using MongoDB.Driver;

namespace LibraryAPI.Services
{
    public class MongoLoanService : IMongoService<LoansMongo, LoanDto, CreateLoanDto, LoanDto>
    {
        private readonly MongoRepository<LoansMongo> _repository;
        private readonly MongoRepository<InventoryMongo> _inventoryRepository;
        private readonly MongoRepository<ItemMongo> _itemRepository;

        public MongoLoanService(MongoDbContext context)
        {
            _repository = new MongoRepository<LoansMongo>(context, "Loans");
            _inventoryRepository = new MongoRepository<InventoryMongo>(context, "Inventory");
            _itemRepository = new MongoRepository<ItemMongo>(context, "Items");
        }
        public async Task<LoanDto> CreateAsync(CreateLoanDto newLoan)
        {
            var inventory = await _inventoryRepository.GetByIdAsync(newLoan.InventoryId);
            var item = await _itemRepository.GetByIdAsync(inventory.Item_Id);

            var loanCreation = await _repository.CreateAsync(new LoansMongo
            {
                _id = ObjectId.GenerateNewId(),
                Id = (await _repository.GetAllAsync()).Count + 1, // Generate a new ID based on the count of existing loans
                Loaner_Id = newLoan.LoanerId,
                InventoryId = newLoan.InventoryId,
                Loan_Date = DateTime.UtcNow,
                Due_Date = DateTime.UtcNow.AddDays(14), // Example due date
                Return_Date = null,
                Status = "active",
                Item_Snapshot = new ItemSnapshot { MediaType = item.MediaType, Name = item.Name }, // Fetch and set the item snapshot from InventoryMongo
                Inventory_Snapshot = new InventorySnapshot { Barcode = inventory.Barcode }, // Fetch and set the inventory snapshot from InventoryMongo
                Fines = new List<FineMongo>() // Initialize with an empty list
            });
            return MapToDto(loanCreation);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try {  return await _repository.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                throw new Exception($"Error deleting loan with ID {id}: {ex.Message}", ex);
            }
        }

        public async Task<List<LoanDto>> GetAllAsync()
        {
            var loans = await _repository.GetAllAsync();
            return loans.Select(MapToDto).ToList();
        }
        public async Task<LoanDto> GetByIdAsync(int id)
        {
            var loan = await _repository.GetByIdAsync(id);
            if (loan == null)
                return null;

            return MapToDto(loan);
        }

        private async Task<string> GetMongoId(int id)
        {
            LoansMongo loan = await _repository.GetByIdAsync(id);
            return loan?._id.ToString();
        }

        public async Task<bool> UpdateAsync(LoanDto loanDto, int id)
        {
            if(await GetByIdAsync(id) != null)
            {
                await _repository.UpdateAsync(id, new LoansMongo
                {
                    _id = ObjectId.Parse(await GetMongoId(id)),
                    Id = loanDto.Id,
                    Loan_Date = loanDto.LoanDate,
                    Due_Date = loanDto.DueDate,
                    Return_Date = loanDto.ReturnDate,
                    Status = loanDto.Status,
                    Loaner_Id = loanDto.LoanerId,
                    InventoryId = loanDto.InventoryId
                });
                return true;
            }
            return false;
        }

        public LoanDto MapToDto(LoansMongo loansMongo)
        {
            // Await the asynchronous call to get the InventoryMongo object
            var invTask = _inventoryRepository.GetByIdAsync(loansMongo.InventoryId);
            invTask.Wait();
            var inv = invTask.Result;

            return new LoanDto
            {
                Id = loansMongo.Id,
                LoanDate = loansMongo.Loan_Date,
                DueDate = loansMongo.Due_Date,
                ReturnDate = loansMongo.Return_Date,
                Status = loansMongo.Status,
                LoanerId = loansMongo.Loaner_Id,
                InventoryId = loansMongo.InventoryId,
                InventoryStatus = inv?.Status // Fetch from InventoryMongo
            }; 
        }
    }
}
