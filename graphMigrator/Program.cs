using graphMigrator;
using MongoDB.Driver;
using Secret = graphMigrator.Secret;


internal class Program
{

    private static async Task Main(string[] args)
    {
        Secret secret = new Secret();
        var mysql = new MySqlService(secret.MySqlConnectionString);

        // MongoDB Migration
        var mongoDB = new MongoDBService(secret.MongoDbConnectionString, secret.MySqlConnectionString);

        await mongoDB.EnsureUsersAndPrivilegesAsync();

        // Ensure collections exist + create indexes (unique constraints)
        await mongoDB.EnsureConstraintsAsync();

        using (var session = mongoDB.StartSession())
        { 
            var transactionOptions = new TransactionOptions(
            readConcern: ReadConcern.Majority,
            writeConcern: WriteConcern.WMajority
            );

            session.StartTransaction(transactionOptions);
            try {
                await mongoDB.ClearCollection("Items");
                await mongoDB.InsertData("Items", mongoDB.TransformItems());
                await mongoDB.ClearCollection("Inventory");
                await mongoDB.InsertData("Inventory", mongoDB.TransformInventory());
                await mongoDB.ClearCollection("Loaners");
                await mongoDB.InsertData("Loaners", mongoDB.TransformLoaners());
                await mongoDB.ClearCollection("Loans");
                await mongoDB.InsertData("Loans", mongoDB.TransformLoans());
                await mongoDB.ClearCollection("Reservations");
                await mongoDB.InsertData("Reservations", mongoDB.TransformReservations());

                await session.CommitTransactionAsync();
                Console.WriteLine("Migration completed successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during migration: {ex.Message}");
                await session.AbortTransactionAsync();
                return;
            }
        }
        }
}