using LibraryAPI.Services.Interfaces;
using MongoDB.Bson;
using MongoDB.Driver;

namespace LibraryAPI.Services
{
    public class MongoLoanService : IMongoService
    {
        private readonly IMongoCollection<LoansMongo> _loans;
        public MongoLoanService(IMongoDatabase database) 
        {
            _loans = database.GetCollection<LoansMongo>("Loans");
        }
        public Task Create()
        {
            throw new NotImplementedException();
        }

        public Task Delete()
        {
            throw new NotImplementedException();
        }

        public Task GetAll()
        {
            throw new NotImplementedException();
        }

        public Task GetOne()
        {
            throw new NotImplementedException();
        }

        public Task Update()
        {
            throw new NotImplementedException();
        }

        public Task MapToDto()
        {
        //    [BsonId]
        //    public ObjectId _id { get; set; }
        //public int Id { get; set; }
        //public int Loaner_Id { get; set; }
        //public int InventoryId { get; set; }

        //public DateTime Loan_Date { get; set; }
        //public DateTime Due_Date { get; set; }
        //public DateTime? Return_Date { get; set; }

        //public string Status { get; set; } // "active" | "overdue" | "returned"
        //public ItemSnapshot Item_Snapshot { get; set; }
        //public InventorySnapshot Inventory_Snapshot { get; set; }
        //public List<FineMongo> Fines { get; set; }
            throw new NotImplementedException();
        }
    }
}
