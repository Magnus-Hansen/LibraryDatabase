using graphMigrator;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var mysql = new MySqlService("server=localhost;user=root;password=123456;database=mydb; port=3306");
        var neo4j = new Neo4jService("bolt://localhost:7687", "neo4j", "bookworm");

        var loaners = mysql.GetLoaners();
        var mapper = new Mapper();

        await mapper.MigrateUsers(loaners, neo4j);
        await mapper.MigrateLanguages(mysql.GetLanguages(), neo4j);

        Console.WriteLine("Completed migration");
    }
}