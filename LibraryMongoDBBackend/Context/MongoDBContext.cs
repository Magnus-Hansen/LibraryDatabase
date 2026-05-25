using MongoDB.Driver;

public class MongoDbContext
{
    public IMongoClient Client { get; }
    private readonly IMongoDatabase _database;

    public MongoDbContext(string connectionString, string dbName)
    {
        Client = new MongoClient(connectionString);
        _database = Client.GetDatabase(dbName);
    }

    public IMongoCollection<ItemMongo> Items =>
        _database.GetCollection<ItemMongo>("Items");

    public IMongoCollection<InventoryMongo> Inventory =>
        _database.GetCollection<InventoryMongo>("Inventory");

    public IMongoCollection<LoanersMongo> Loaners =>
        _database.GetCollection<LoanersMongo>("Loaners");

    public IMongoCollection<LoansMongo> Loans =>
        _database.GetCollection<LoansMongo>("Loans");

    public IMongoCollection<ReservationsMongo> Reservations =>
        _database.GetCollection<ReservationsMongo>("Reservations");
}