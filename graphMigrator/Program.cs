using graphMigrator;
using Neo4j.Driver;
using MongoDB.Driver;
using Secret = graphMigrator.Secret;


internal class Program
{

    private static async Task Main(string[] args)
    {
        Secret secret = new Secret();
        var mysql = new MySqlService(secret.MySqlConnectionString);
        var neo4j = new Neo4jService(secret.Neo4jUri, secret.Neo4jUser, secret.Neo4jPassword, secret.Neo4jDatabase);
        var neo4jQueries = neo4j.nodeQueries;
        var neo4jConstraints = neo4j.Constraint;
        var neo4jIndexes = neo4j.Indexes;
        var NodeId = neo4j.NodeId;

        await neo4j.ExecuteInTransaction(async transaction =>
        {
            await neo4j.DeleteConstraintIndex(transaction);
        });
        await neo4j.ExecuteInTransaction(async transaction =>
        {
            foreach (var constraint in neo4jConstraints.Values)
            {
                await neo4j.Neo4jExecute(transaction, constraint);
            }
            foreach (var query in neo4jIndexes.Values)
            {
                await neo4j.Neo4jExecute(transaction, query);
            }
        });
        await neo4j.ExecuteInTransaction(async transaction =>
        {
            await neo4j.DeleteNodes(transaction);
            await neo4j.Neo4jExecute(transaction, mysql.GetLoaners(), neo4jQueries["Loaner"]);
            await neo4j.Neo4jExecute(transaction, mysql.GetLanguages(), neo4jQueries["Language"]);
            await neo4j.Neo4jExecute(transaction, mysql.GetItems(), neo4jQueries["Item"]);
            await neo4j.Neo4jExecute(transaction, mysql.GetCreators(), neo4jQueries["Creator"]);
            await neo4j.Neo4jExecute(transaction, mysql.GetBooks(), neo4jQueries["Book"]);
            await neo4j.Neo4jExecute(transaction, mysql.GetPublishers(), neo4jQueries["Publisher"]);
            await neo4j.Neo4jExecute(transaction, mysql.GetGenres(), neo4jQueries["Genre"]);
            await neo4j.Neo4jExecute(transaction, mysql.GetTags(), neo4jQueries["Tag"]);
            await neo4j.Neo4jExecute(transaction, mysql.GetInventories(), neo4jQueries["Inventory"]);
            await neo4j.Neo4jExecute(transaction, mysql.GetLoans(), neo4jQueries["Loan"]);
            await neo4j.Neo4jExecute(transaction, mysql.GetReservations(), neo4jQueries["Reservation"]);
            await neo4j.Neo4jExecute(transaction, mysql.GetFines(), neo4jQueries["Fine"]);
            await neo4j.Neo4jExecute(transaction, mysql.GetBoardGames(), neo4jQueries["Boardgame"]);
            await neo4j.Neo4jExecute(transaction, mysql.GetItems(), neo4jQueries["Item_Language"]);
            await neo4j.Neo4jExecute(transaction, mysql.GetItems(), neo4jQueries["Item_Publisher"]);
            await neo4j.Neo4jExecute(transaction, mysql.GetItemCreators(), neo4jQueries["Item_Creator"]);
            await neo4j.Neo4jExecute(transaction, mysql.GetBooks(), neo4jQueries["Book_Item"]);
            await neo4j.Neo4jExecute(transaction, mysql.GetBoardGames(), neo4jQueries["Boardgame_Item"]);
            await neo4j.Neo4jExecute(transaction, mysql.GetItemGenres(), neo4jQueries["Item_Genre"]);
            await neo4j.Neo4jExecute(transaction, mysql.GetItemTags(), neo4jQueries["Item_Tag"]);
            await neo4j.Neo4jExecute(transaction, mysql.GetReviews(), neo4jQueries["Review"]);
            await neo4j.Neo4jExecute(transaction, mysql.GetReservations(), neo4jQueries["Item_Reservation"]);
            await neo4j.Neo4jExecute(transaction, mysql.GetInventories(), neo4jQueries["Item_Inventory"]);
            await neo4j.Neo4jExecute(transaction, mysql.GetReservations(), neo4jQueries["Loaner_Reservation"]);
            await neo4j.Neo4jExecute(transaction, mysql.GetLoans(), neo4jQueries["Loaner_Loan"]);
            await neo4j.Neo4jExecute(transaction, mysql.GetFines(), neo4jQueries["Fine_Loan"]);
            await neo4j.Neo4jExecute(transaction, mysql.GetLoans(), neo4jQueries["Loan_Inventory"]);
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
            foreach (var nodeId in NodeId.Values)
            {
                await neo4j.Neo4jExecute(transaction, nodeId);
            }

            Console.WriteLine("Completed migration");
        });
    }
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