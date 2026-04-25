using graphMigrator;
using Neo4j.Driver;

internal class Program
{
    private static async Task Main(string[] args)
    {
        Secret secret = new Secret();
        var mysql = new MySqlService(secret.MySqlConnectionString);
        var neo4j = new Neo4jService(secret.Neo4jUri, secret.Neo4jUser, secret.Neo4jPassword, secret.Neo4jDatabase);

        await neo4j.ExecuteInTransaction(async transaction =>
        {
            await neo4j.DeleteEverything(transaction);
            await neo4j.CreateLoaner(transaction, mysql.GetLoaners());
            await neo4j.CreateLanguage(transaction, mysql.GetLanguages());
            await neo4j.CreateItem(transaction, mysql.GetItems());
            await neo4j.CreateCreator(transaction, mysql.GetCreators());
            await neo4j.CreateBook(transaction, mysql.GetBooks());
            await neo4j.CreatePublisher(transaction, mysql.GetPublishers());
            await neo4j.CreateGenre(transaction, mysql.GetGenres());
            await neo4j.CreateTag(transaction, mysql.GetTags());
            await neo4j.CreateInventory(transaction, mysql.GetInventories());
            await neo4j.CreateLoan(transaction, mysql.GetLoans());
            await neo4j.CreateReservation(transaction, mysql.GetReservations());
            await neo4j.CreateFine(transaction, mysql.GetFines());
            await neo4j.CreateBoardGame(transaction, mysql.GetBoardGames());
            await neo4j.Item_Language(transaction, mysql.GetItems());
            await neo4j.Item_Publisher(transaction, mysql.GetItems());
            await neo4j.Item_Creator(transaction, mysql.GetItemCreators());
            await neo4j.Book_item(transaction, mysql.GetBooks());
            await neo4j.Item_Genre(transaction, mysql.GetItemGenres());
            await neo4j.Item_Tag(transaction, mysql.GetItemTags());
            await neo4j.Boardgame_item(transaction, mysql.GetBoardGames());
            await neo4j.CreateReview(transaction, mysql.GetReviews());
            await neo4j.Item_Inventory(transaction, mysql.GetInventories());
            await neo4j.Item_Reservation(transaction, mysql.GetReservations());
            await neo4j.Loaner_Loan(transaction, mysql.GetLoans());
            await neo4j.Loaner_Reservation(transaction, mysql.GetReservations());
            await neo4j.Loan_Fine(transaction, mysql.GetFines());
            await neo4j.Loan_Inventory(transaction, mysql.GetLoans());


            Console.WriteLine("Completed migration");
        });
    }
}