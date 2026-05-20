namespace graphMigrator;

public class Secret
{
    public string MySqlConnectionString { get; set; } =
        "Server=localhost;Port=3306;Database=mydb;User=root;Password=123456;";

    public string MongoDbConnectionString { get; set; } =
        "mongodb://admin:secret@localhost:27017/mydatabase?authSource=admin";
}