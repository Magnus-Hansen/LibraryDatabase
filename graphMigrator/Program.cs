using graphMigrator;
using Microsoft.AspNetCore.DataProtection;

internal class Program
{
    private static async Task Main(string[] args)
    {
        Secret secret = new Secret();
        var mysql = new MySqlService(secret.MySqlConnectionString);
        var neo4j = new Neo4jService(secret.Neo4jUri, secret.Neo4jUser, secret.Neo4jPassword, secret.Neo4jDatabase);

        await neo4j.DeleteEverything();
        await neo4j.CreateLoaner(mysql.GetLoaners());
        await neo4j.CreateLanguage(mysql.GetLanguages());
        await neo4j.CreateItem(mysql.GetItems());
        await neo4j.CreateCreator(mysql.GetCreators());
        await neo4j.CreateBook(mysql.GetBooks());
        await neo4j.CreatePublisher(mysql.GetPublishers());
        await neo4j.CreateGenre(mysql.GetGenres());
        await neo4j.CreateTag(mysql.GetTags());
        await neo4j.CreateInventory(mysql.GetInventories());
        await neo4j.CreateLoan(mysql.GetLoans());
        await neo4j.CreateReservation(mysql.GetReservations());
        await neo4j.CreateFine(mysql.GetFines());
        await neo4j.CreateBoardGame(mysql.GetBoardGames());
        await neo4j.Item_Language(mysql.GetItems());
        await neo4j.Item_Publisher(mysql.GetItems());
        await neo4j.Item_Creator(mysql.GetItemCreators());
        await neo4j.Book_item(mysql.GetBooks());
        await neo4j.Item_Genre(mysql.GetItemGenres());
        await neo4j.Item_Tag(mysql.GetItemTags());
        await neo4j.Boardgame_item(mysql.GetBoardGames());
        await neo4j.CreateReview(mysql.GetReviews());
        await neo4j.Item_Inventory(mysql.GetInventories());
        await neo4j.Item_Reservation(mysql.GetReservations());
        await neo4j.Loaner_Loan(mysql.GetLoans());
        await neo4j.Loaner_Reservation(mysql.GetReservations());
        await neo4j.Loan_Fine(mysql.GetFines());
        await neo4j.Loan_Inventory(mysql.GetLoans());


        Console.WriteLine("Completed migration");
    }
}