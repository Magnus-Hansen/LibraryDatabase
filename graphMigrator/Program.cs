using graphMigrator;

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

        Console.WriteLine("Completed migration");
    }
}