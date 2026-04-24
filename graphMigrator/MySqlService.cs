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
                    First_name = reader.GetString("first_name"),
                    Last_name = reader.GetString("last_name"),
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
        public List<Item> GetItems()
        {
            List<Item> items = new List<Item>();
            using var connection = new MySqlConnection(_connectionString);
            connection.Open();

            using var cmd = new MySqlCommand("SELECT * FROM item", connection);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
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
        public List<Creator> GetCreators()
        {
            List<Creator> creators = new List<Creator>();
            using var connection = new MySqlConnection(_connectionString);
            connection.Open();
            using var cmd = new MySqlCommand("SELECT * FROM creator", connection);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                creators.Add(new Creator
                {
                    Id = reader.GetInt32("id"),
                    First_name = reader.GetString("first_name"),
                    Last_name = reader.GetString("last_name"),
                    Birthday = reader.IsDBNull("birthday") ? null : reader.GetDateTime("birthday"),
                    Description = reader.IsDBNull("description") ? null : reader.GetString("description")
                });
            }
            return creators;
        }
        public List<Publisher> GetPublishers()
        {
            List<Publisher> publishers = new List<Publisher>();
            using var connection = new MySqlConnection(_connectionString);
            connection.Open();
            using var cmd = new MySqlCommand("SELECT * FROM publisher", connection);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                publishers.Add(new Publisher
                {
                    Id = reader.GetInt32("id"),
                    Name = reader.GetString("name")
                });
            }
            return publishers;
        }
        public List<Book> GetBooks()
        {
            List<Book> books = new List<Book>();
            using var connection = new MySqlConnection(_connectionString);
            connection.Open();
            using var cmd = new MySqlCommand("SELECT * FROM book", connection);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                books.Add(new Book
                {
                    Id = reader.GetInt32("id"),
                    ISBN = reader.GetString("ISBN"),
                    No_of_pages = reader.GetInt32("no_of_pages"),
                    Version = reader.IsDBNull("version") ? null : reader.GetString("version"),
                    Item_id = reader.GetInt32("item_id")
                });
            }
            return books;
        }
        public List<Genre> GetGenres()
        {
            List<Genre> genres = new List<Genre>();
            using var connection = new MySqlConnection(_connectionString);
            connection.Open();
            using var cmd = new MySqlCommand("SELECT * FROM genre", connection);
            using var readers = cmd.ExecuteReader();
            while (readers.Read())
            {
                genres.Add(new Genre 
                { 
                    Id = readers.GetInt32("id"), 
                    Name = readers.GetString("name") 
                });
            }
            return genres;
        }
        public List<Tag> GetTags()
        {
            List<Tag> tags = new List<Tag>();
            using var connection = new MySqlConnection(_connectionString);
            connection.Open();
            using var cmd = new MySqlCommand("SELECT * FROM tag", connection);
            using var readers = cmd.ExecuteReader();
            while (readers.Read())
            {
                tags.Add(new Tag { 
                    Id = readers.GetInt32("id"), 
                    Name = readers.GetString("name") 
                });
            }
            return tags;
        }
        public List<Inventory> GetInventories()
        {
            List<Inventory> inventories = new List<Inventory>();
            using var connection = new MySqlConnection(_connectionString);
            connection.Open();
            using var cmd = new MySqlCommand("SELECT * FROM inventory", connection);
            using var readers = cmd.ExecuteReader();
            while (readers.Read())
            {
                inventories.Add(new Inventory
                {
                    Id = readers.GetInt32("id"),
                    Item_id = readers.GetInt32("item_id"),
                    Status = readers.GetString("status"),
                    Barcode = readers.GetString("barcode"),
                    Placement = readers.IsDBNull("placement") ? null : readers.GetString("placement")
                });
            }
            return inventories;
        }
        public List<Loan> GetLoans()
        {
            List<Loan> loans = new List<Loan>();
            using var connection = new MySqlConnection(_connectionString);
            connection.Open();
            using var cmd = new MySqlCommand("SELECT * FROM loan", connection);
            using var readers = cmd.ExecuteReader();
            while (readers.Read())
            {
                loans.Add(new Loan
                {
                    Id = readers.GetInt32("id"),
                    Loaner_id = readers.GetInt32("loaner_id"),
                    Inventory_id = readers.GetInt32("inventory_id"),
                    Loan_date = readers.GetDateTime("loan_date"),
                    Due_date = readers.GetDateTime("due_date"),
                    Return_date = readers.IsDBNull("return_date") ? null : readers.GetDateTime("return_date"),
                    Status = readers.GetString("status")
                });
            }
            return loans;
        }
        public List<Reservation> GetReservations()
        {
            List<Reservation> reservations = new List<Reservation>();
            using var connection = new MySqlConnection(_connectionString);
            connection.Open();
            using var cmd = new MySqlCommand("SELECT * FROM reservation", connection);
            using var readers = cmd.ExecuteReader();
            while (readers.Read())
            {
                reservations.Add(new Reservation
                {
                    Id = readers.GetInt32("id"),
                    Loaner_id = readers.GetInt32("loaner_id"),
                    Item_id = readers.GetInt32("item_id"),
                    Status = readers.GetString("status"),
                    Queue_number = readers.GetInt32("queue_number")
                });
            }
            return reservations;
        }
        public List<Review> GetReviews()
        {
            List<Review> reviews = new List<Review>();
            using var connection = new MySqlConnection(_connectionString);
            connection.Open();
            using var cmd = new MySqlCommand("SELECT * FROM review", connection);
            using var readers = cmd.ExecuteReader();
            while (readers.Read())
            {
                reviews.Add(new Review
                {
                    Loaner_id = readers.GetInt32("loaner_id"),
                    Item_id = readers.GetInt32("item_id"),
                    No_of_stars = readers.GetInt32("no_of_stars"),
                    Text = readers.IsDBNull("text") ? null : readers.GetString("text")
                });
            }
            return reviews;
        }
        public List<Fine> GetFines()
        {
            List<Fine> fines = new List<Fine>();
            using var connection = new MySqlConnection(_connectionString);
            connection.Open();
            using var cmd = new MySqlCommand("SELECT * FROM fine", connection);
            using var readers = cmd.ExecuteReader();
            while (readers.Read())
            {
                fines.Add(new Fine
                {
                    Id = readers.GetInt32("id"),
                    Amount = readers.GetFloat("amount"),
                    Status = readers.GetString("status"),
                    Created_date = readers.GetDateTime("created_date"),
                    Paid_date = readers.IsDBNull("paid_date") ? null : readers.GetDateTime("paid_date"),
                    Due_date = readers.GetDateTime("due_date"),
                    Loan_id = readers.GetInt32("loan_id")
                });
            }
            return fines;
        }
    }
}
