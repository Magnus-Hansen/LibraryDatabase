using MongoDB.Driver;

public class MongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext(string connectionString, string dbName)
    {
        var client = new MongoClient(connectionString);
        _database = client.GetDatabase(dbName);
    }

    public IMongoCollection<ItemMongo> Items =>
        _database.GetCollection<ItemMongo>("Items");

    public IMongoCollection<InventoryMongo> Inventories =>
        _database.GetCollection<InventoryMongo>("Inventory");

    public IMongoCollection<LoanersMongo> Loaners =>
        _database.GetCollection<LoanersMongo>("Loaners");

    public IMongoCollection<LoansMongo> Loans =>
        _database.GetCollection<LoansMongo>("Loans");

    public IMongoCollection<ReservationsMongo> Reservations =>
        _database.GetCollection<ReservationsMongo>("Reservations");
}