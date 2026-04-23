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
    }
}
