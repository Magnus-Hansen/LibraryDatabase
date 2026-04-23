using graphMigrator.Models;
using MySqlConnector;
using System.Data;

namespace graphMigrator
{
    public class MySqlService
    {
        private readonly string _connectionString;

        public MySqlService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<Loaner> GetLoaners()
        {
            List<Loaner> loaners = new List<Loaner>();
            using var conn = new MySqlConnection(_connectionString);
            conn.Open();

            var cmd = new MySqlCommand("SELECT * FROM loaner", conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                loaners.Add(new Loaner
                {
                    Id = reader.GetInt32("id"),
                    FirstName = reader.GetString("first_name"),
                    LastName = reader.GetString("last_name"),
                    CPR = reader.GetString("cpr"),
                    Tlf = reader.IsDBNull("tlf") ? null : reader.GetString("tlf"),
                    Email = reader.GetString("email"),
                    Password = reader.GetString("password")
                });
            }
            return loaners;
        }
        public List<Language> GetLanguages()
        {
            List<Language> languages = new List<Language>();
            using var conn = new MySqlConnection(_connectionString);
            conn.Open();

            var cmd = new MySqlCommand("SELECT * FROM language", conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                languages.Add(new Language
                {
                    Id = reader.GetInt32("id"),
                    Name = reader.GetString("language")
                });
            }
            return languages;
        }
        public List<Item> GetItems() {
            List<Item> items = new List<Item>();
            using var connection = new MySqlConnection(_connectionString);
            connection.Open();

            using var cmd = new MySqlCommand("SELECT * FROM Item", connection);
            using var reader = cmd.ExecuteReader();
            while (reader.Read()) {
                items.Add(new Item
                {
                    Language_id = reader.GetInt32("id"),
                    Name = reader.GetString("name"),
                    Release_year = reader.GetInt16("release_year"),
                    Description = reader.GetString("description"),
                    Review_summary = reader.GetString("review_summary"),
                    Media_type = reader.GetString("media_type"),
                    Image = reader.GetString("image"),
                    Publisher_id = reader.GetInt32("publisher_id"),
                    Average_stars = reader.GetFloat("average_stars")
                });
            }
            return items;
        }
    }
}
