//using LibraryMongoDBBackend.Repositories.Interfaces;
//using MongoDB.Driver;

//public class LoanService
//{
//    private readonly IRepository<LoansMongo> _loanRepo;
//    private readonly IRepository<InventoryMongo> _inventoryRepo;
//    private readonly IMongoClient _client;

//    public LoanService(
//        IRepository<LoansMongo> loanRepo,
//        IRepository<InventoryMongo> inventoryRepo,
//        IMongoClient client)
//    {
//        _loanRepo = loanRepo;
//        _inventoryRepo = inventoryRepo;
//        _client = client;
//    }

//    public async Task<bool> CreateLoan(string barcode, int loanerId)
//    {
//        using var session = await _client.StartSessionAsync();
//        session.StartTransaction();

//        try
//        {
//            // 1. Get inventory item
//            var inventory = (await _inventoryRepo.GetAllAsync())
//                .FirstOrDefault(x => x.Barcode == barcode);

//            if (inventory == null || inventory.Status != "Available")
//                return false;

//            // 2. Create loan
//            var loan = new LoansMongo
//            {
//                InventoryId = inventory.Item_Id,
//                Loaner_Id = loanerId,
//                Loan_Date = DateTime.UtcNow,
//                Due_Date = DateTime.UtcNow.AddDays(14),
//                Status = "Active"
//            };

//            await _loanRepo.CreateAsync(loan);

//            // 3. Update inventory
//            inventory.Status = "Loaned";
//            await _inventoryRepo.UpdateAsync(inventory.Id, inventory);

//            await session.CommitTransactionAsync();
//            return true;
//        }
//        catch
//        {
//            await session.AbortTransactionAsync();
//            throw;
//        }
//    }
//}