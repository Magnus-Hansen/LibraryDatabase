using graphMigrator;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using Secret = graphMigrator.Secret;


internal class Program
{

    private static async Task Main(string[] args)
    {
        Secret secret = new Secret();
        var mysql = new MySqlService(secret.MySqlConnectionString);

        // MongoDB Migration
        var mongoDB = new MongoDBService(secret.MongoDbConnectionString, secret.MySqlConnectionString);
        await mongoDB.ClearCollection("Items");
        await mongoDB.InsertData("Items", mongoDB.TransformItems());
        await mongoDB.ClearCollection("Inventories");
        await mongoDB.InsertData("Inventories", mongoDB.TransformInventory());
        await mongoDB.ClearCollection("Loaners");
        await mongoDB.InsertData("Loaners", mongoDB.TransformLoaners());
        await mongoDB.ClearCollection("Loans");
        await mongoDB.InsertData("Loans", mongoDB.TransformLoans());
        await mongoDB.ClearCollection("Reservations");
        await mongoDB.InsertData("Reservations", mongoDB.TransformReservations());

        await mongoDB.CreateIndexes();


    }
}