using graphMigrator;

internal class Program
{
    private static async Task Main(string[] args)
    {
        Secret secret = new Secret();
        var mysql = new MySqlService(secret.MySqlConnectionString);
        var neo4j = new Neo4jService(secret.Neo4jUri, secret.Neo4jUser, secret.Neo4jPassword, secret.Neo4jDatabase);
        var mapper = new Mapper();

        await mapper.MigrateUsers(mysql.GetLoaners(), neo4j);
        await mapper.MigrateLanguages(mysql.GetLanguages(), neo4j);
        await mapper.MigrateItems(mysql.GetItems(), neo4j);

        Console.WriteLine("Completed migration");
    }
}