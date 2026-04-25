using graphMigrator.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tag = graphMigrator.Models.Tag;

namespace graphMigrator
{
    public class MongoDBService
    {
        private readonly MongoClient _client;
        private readonly IMongoDatabase _database;

        private readonly string _databaseName = "LibraryMongo";

        MySqlService mysqlService;
        public MongoDBService(string connectionString, string mysqlconnectionString)
        {
            _client = new MongoClient(connectionString);
            _database = _client.GetDatabase(_databaseName);

            mysqlService = new MySqlService(mysqlconnectionString);
        }

        public string Connect()
        {
            try
            {
                // Connection check
                _database.RunCommandAsync<BsonDocument>(
                new BsonDocument("ping", 1)
                );

                return "Connection to MongoDB successful!";
            }
            catch (Exception ex)
            {
                return $"Failed to connect to MongoDB: {ex.Message}";
            }
        }

        public IMongoDatabase Database => _database;

        // Extract data from MySQL
        public List<T> ExtractData<T>(string sqlQuery)
        {
            // Implement MySQL data extraction logic here
            // This is a placeholder for demonstration purposes
            return new List<T>();
        }

        // Transform data to MongoDB models
        public bool TransformData()
        {
            try
            {
                List<ItemMongo> items = TransformItems();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to transform data: {ex.Message}");
                return false;
            }
            

            
        }

        // Transforms items from MySQL to MongoDB format
        public List<ItemMongo> TransformItems()
        {
            // Turns MySQL Language into MongoDB LanguageMongo
            List<Language> languages = mysqlService.GetLanguages();
            List<LanguageMongo> languageMongos = new List<LanguageMongo>();
            foreach (Language language in languages) {
                LanguageMongo languageMongo = new LanguageMongo
                {
                    Id = language.Id,
                    Name = language.Name
                };
                languageMongos.Add(languageMongo);
            }
            // Turns MySQL Publisher into MongoDB PublisherMongo
            List<Publisher> publishers = mysqlService.GetPublishers();
            List<PublisherMongo> publisherMongos = new List<PublisherMongo>();
            foreach (Publisher publisher in publishers)
            {
                PublisherMongo publisherMongo = new PublisherMongo
                {
                    Id = publisher.Id,
                    Name = publisher.Name
                };
                publisherMongos.Add(publisherMongo);
            }
            // Turns MySQL Creator into MongoDB CreatorMongo
            List<Creator> creators = mysqlService.GetCreators();
            List<CreatorMongo> creatorMongos = new List<CreatorMongo>();
            foreach (Creator creator in creators)
            {
                CreatorMongo creatorMongo = new CreatorMongo
                {
                    Id = creator.Id,
                    FirstName = creator.First_name,
                    LastName = creator.Last_name,
                    Birthday = creator.Birthday,
                    Description = creator.Description ?? ""

                };
                creatorMongos.Add(creatorMongo);
            }
            List<ItemCreator> itemCreators = mysqlService.GetItemCreators();
            // Turns MySQL Genre into MongoDB GenreMongo
            List<Genre> genres = mysqlService.GetGenres();
            List<GenreMongo> genreMongos = new List<GenreMongo>();
            foreach (Genre genre in genres)
            {
                GenreMongo genreMongo = new GenreMongo
                {
                    Id = genre.Id,
                    Name = genre.Name
                };
                genreMongos.Add(genreMongo);
            }
            List<ItemGenre> itemGenres = mysqlService.GetItemGenres();
            // Turns MySQL Tag into MongoDB TagMongo
            List<Tag> tags = mysqlService.GetTags();
            List<TagMongo> tagMongos = new List<TagMongo>();
            foreach (Tag tag in tags)
            {
                TagMongo tagMongo = new TagMongo
                {
                    Id = tag.Id,
                    Name = tag.Name
                };
                tagMongos.Add(tagMongo);
            }
            List<ItemTag> itemTags = mysqlService.GetItemTags();
            // Turns MySQL BookDetails into MongoDB BookDetailsMongo
            List<Book> books = mysqlService.GetBooks();
            List<BookDetailsMongo> bookDetailsMongos = new List<BookDetailsMongo>();
            foreach (Book book in books)
            {
                BookDetailsMongo bookDetailsMongo = new BookDetailsMongo
                {
                    ISBN = book.ISBN,
                    No_Of_Pages = book.No_of_pages,
                    Version = book.Version ?? ""
                };
                bookDetailsMongos.Add(bookDetailsMongo);
            }
            // Turns MySQL BoardgameDetails into MongoDB BoardgameDetailsMongo
            List<BoardGame> boardgameDetails = mysqlService.GetBoardGames();
            List<BoardgameDetailsMongo> boardgameDetailsMongos = new List<BoardgameDetailsMongo>();
            foreach (BoardGame boardgameDetail in boardgameDetails)
            {
                BoardgameDetailsMongo boardgameDetailsMongo = new BoardgameDetailsMongo
                {
                    No_Of_Players = int.TryParse(boardgameDetail.No_of_players, out int noOfPlayers) ? noOfPlayers : 0,
                    Play_Time = int.TryParse(boardgameDetail.Play_time, out int playTime) ? playTime : 0,
                    Age_Group = int.TryParse(boardgameDetail.Age_group, out int ageGroup) ? ageGroup : 0,
                    item_id = boardgameDetail.Item_id
                };
                boardgameDetailsMongos.Add(boardgameDetailsMongo);
            }
            // Turns MySQL Review into MongoDB ReviewMongo and also adds the Loaner name to the ReviewMongo
            List<Review> reviews = mysqlService.GetReviews();
            List<Loaner> loaners = mysqlService.GetLoaners();
            List<ReviewMongo> reviewMongos = new List<ReviewMongo>();
            foreach (Review review in reviews)
            {
                ReviewMongo reviewMongo = new ReviewMongo
                {
                    Review_Id = ObjectId.GenerateNewId(),
                    Loaner_Id = review.Loaner_id,
                    Loaner_Name = loaners.FirstOrDefault(l => l.Id == review.Loaner_id)?.First_name + " " + loaners.FirstOrDefault(l => l.Id == review.Loaner_id)?.Last_name,
                    No_Of_Stars = (int)review.No_of_stars,
                    Text = review.Text ?? "",

                    // object_id is the id of the item that the review is for, this will be used to link the review to the item in MongoDB
                    object_id = review.Item_id
                };
                reviewMongos.Add(reviewMongo);
            }

            List<Item> mySqlItems = mysqlService.GetItems();
            List<ItemMongo> itemMongos = new List<ItemMongo>();

            foreach (Item item in mySqlItems)
            {
                ItemMongo itemMongo = new ItemMongo
                {
                    Id = ObjectId.GenerateNewId(),
                    Name = item.Name,
                    MediaType = item.Media_type,
                    ReleaseYear = item.Release_year ?? 0,
                    Description = item.Description ?? "",
                    ReviewSummary = item.Review_summary ?? "",
                    Image = item.Image ?? "",

                    Language = languageMongos.FirstOrDefault(l => l.Id == item.Language_id),

                    Publisher = publisherMongos.FirstOrDefault(p => p.Id == item.Publisher_id),

                    Creators = itemCreators.Where(ic => ic.Item_id == item.Id)
                        .Select(ic => creatorMongos.FirstOrDefault(c => c.Id == ic.Creator_id))
                        .Where(c => c != null)
                        .ToList(),

                    Genres = itemGenres.Where(ig => ig.Item_id == item.Id)
                        .Select(ig => genreMongos.FirstOrDefault(g => g.Id == ig.Genre_id))
                        .Where(g => g != null)
                        .ToList(),

                    Tags = itemTags.Where(it => it.Item_id == item.Id)
                        .Select(it => tagMongos.FirstOrDefault(t => t.Id == it.Tag_id))
                        .Where(t => t != null)
                        .ToList(),

                    BookDetails = bookDetailsMongos.Where(b => b.ISBN == books.FirstOrDefault(book => book.Item_id == item.Id)?.ISBN).FirstOrDefault(),

                    BoardgameDetails = boardgameDetailsMongos.Where(b => b.item_id == item.Id).FirstOrDefault(),

                    Reviews = reviewMongos.Where(r => r.object_id == item.Id).ToList(),
                    AverageStars = item.Average_stars ?? 0.0
                };


            }

            return itemMongos;
        }

        // Transforms Inventory from MySQL to MongoDB format
        //[BsonId]
        //public ObjectId item_id { get; set; }

        //public string item_name { get; set; }
        //public string barcode { get; set; }
        //public string status { get; set; } // "available" | "loaned out" | "lost" Make enum?
        //public string placement { get; set; }
        public List<InventoryMongo> TransformInventory()
        {
            List<Inventory> inventories = mysqlService.GetInventories();
            List<InventoryMongo> inventoryMongos = new List<InventoryMongo>();
            foreach (Inventory inventory in inventories)
            {
                InventoryMongo inventoryMongo = new InventoryMongo
                {
                    Id = ObjectId.GenerateNewId(),
                    Item_Id = inventory.Item_id,
                    Barcode = inventory.Barcode,
                    Status = inventory.Status,
                    Placement = inventory.Placement ?? ""
                };
                inventoryMongos.Add(inventoryMongo);
            }
            return inventoryMongos;
        }

        // Insert Data into MongoDB
        public string InsertData<T>(string collectionName, List<T> data)
        {
            try
            {
                var collection = _database.GetCollection<T>(collectionName);
                collection.InsertMany(data);
                return $"Data inserted into MongoDB collection '{collectionName}' successfully!";
            }
            catch (Exception ex)
            {
                return $"Failed to insert data into MongoDB: {ex.Message}";
            }
        }
        // Clear existing data in MongoDB collection
        public string ClearCollection(string collectionName)
        {
            try
            {
                var client = new MongoClient(_connectionString);
                var database = client.GetDatabase(_databaseName);
                var collection = database.GetCollection<dynamic>(collectionName);
                collection.DeleteMany(Builders<dynamic>.Filter.Empty);
                return $"Collection '{collectionName}' cleared successfully!";
            }
            catch (Exception ex)
            {
                return $"Failed to clear collection '{collectionName}': {ex.Message}";
            }
        }
        // Load data into MongoDB
        public string LoadData<T>(string collectionName, List<T> data)
        {
            try
            {
                var client = new MongoClient(_connectionString);
                var database = client.GetDatabase(_databaseName);
                var collection = database.GetCollection<T>(collectionName);
                collection.InsertMany(data);
                return $"Data loaded into MongoDB collection '{collectionName}' successfully!";
            }
            catch (Exception ex)
            {
                return $"Failed to load data into MongoDB: {ex.Message}";
            }
        }


    }
}
