using graphMigrator;
using Neo4j.Driver;

internal class Program
{
    private static async Task Main(string[] args)
    {
        Secret secret = new Secret();
        var mysql = new MySqlService(secret.MySqlConnectionString);
        var neo4j = new Neo4jService(secret.Neo4jUri, secret.Neo4jUser, secret.Neo4jPassword, secret.Neo4jDatabase);
        var neo4jQueries = neo4j.nodeQueries;

        await neo4j.ExecuteInTransaction(async transaction =>
        {
            await neo4j.DeleteEverything(transaction);
            await neo4j.Neo4jTransaction(transaction, mysql.GetLoaners(), neo4jQueries["Loaner"]);
            await neo4j.Neo4jTransaction(transaction, mysql.GetLanguages(), neo4jQueries["Language"]);
            await neo4j.Neo4jTransaction(transaction, mysql.GetItems(), neo4jQueries["Item"]);
            await neo4j.Neo4jTransaction(transaction, mysql.GetCreators(), neo4jQueries["Creator"]);
            await neo4j.Neo4jTransaction(transaction, mysql.GetBooks(), neo4jQueries["Book"]);
            await neo4j.Neo4jTransaction(transaction, mysql.GetPublishers(), neo4jQueries["Publisher"]);
            await neo4j.Neo4jTransaction(transaction, mysql.GetGenres(), neo4jQueries["Genre"]);
            await neo4j.Neo4jTransaction(transaction, mysql.GetTags(), neo4jQueries["Tag"]);
            await neo4j.Neo4jTransaction(transaction, mysql.GetInventories(), neo4jQueries["Inventory"]);
            await neo4j.Neo4jTransaction(transaction, mysql.GetLoans(), neo4jQueries["Loan"]);
            await neo4j.Neo4jTransaction(transaction, mysql.GetReservations(), neo4jQueries["Reservation"]);
            await neo4j.Neo4jTransaction(transaction, mysql.GetFines(), neo4jQueries["Fine"]);
            await neo4j.Neo4jTransaction(transaction, mysql.GetBoardGames(), neo4jQueries["Boardgame"]);
            await neo4j.Neo4jTransaction(transaction, mysql.GetItems(), neo4jQueries["Item_Language"]);
            await neo4j.Neo4jTransaction(transaction, mysql.GetItems(), neo4jQueries["Item_Publisher"]);
            await neo4j.Neo4jTransaction(transaction, mysql.GetItemCreators(), neo4jQueries["Item_Creator"]);
            await neo4j.Neo4jTransaction(transaction, mysql.GetBooks(), neo4jQueries["Book_Item"]);
            await neo4j.Neo4jTransaction(transaction, mysql.GetBoardGames(), neo4jQueries["Boardgame_Item"]);
            await neo4j.Neo4jTransaction(transaction, mysql.GetItemGenres(), neo4jQueries["Item_Genre"]);
            await neo4j.Neo4jTransaction(transaction, mysql.GetItemTags(), neo4jQueries["Item_Tag"]);
            await neo4j.Neo4jTransaction(transaction, mysql.GetReviews(), neo4jQueries["Review"]);
            await neo4j.Neo4jTransaction(transaction, mysql.GetReservations(), neo4jQueries["Item_Reservation"]);
            await neo4j.Neo4jTransaction(transaction, mysql.GetInventories(), neo4jQueries["Item_Inventory"]);
            await neo4j.Neo4jTransaction(transaction, mysql.GetReservations(), neo4jQueries["Loaner_Reservation"]);
            await neo4j.Neo4jTransaction(transaction, mysql.GetLoans(), neo4jQueries["Loaner_Loan"]);
            await neo4j.Neo4jTransaction(transaction, mysql.GetFines(), neo4jQueries["Fine_Loan"]);
            await neo4j.Neo4jTransaction(transaction, mysql.GetLoans(), neo4jQueries["Loan_Inventory"]);

            Console.WriteLine("Completed migration");
        });
    }
}