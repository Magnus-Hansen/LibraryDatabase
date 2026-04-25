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


        // Transform data to MongoDB models
        public bool TransformData()
        {
            try
            {
                List<ItemMongo> items = TransformItems();
                List<InventoryMongo> inventories = TransformInventory();
                List<LoanersMongo> loaners = TransformLoaners();
                List<LoansMongo> loans = TransformLoans();
                List<ReservationsMongo> reservations = TransformReservations();
                Console.WriteLine("Succesfully transformed all data");
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

            // Turns MySQL Item into MongoDB ItemMongo and also adds the Language, Publisher, Creators, Genres, Tags, BookDetails, BoardgameDetails and Reviews to the ItemMongo
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
                itemMongos.Add(itemMongo);
            }

            return itemMongos;
        }

        // Transforms Inventory from MySQL to MongoDB format
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

        // Transforms active loam from MySQL to MongoDB format, where id is the loaner id, used in TransformLoaners
        public List<ActiveLoansPreviewMongo> TransformActiveLoansPreview(int loanerId)
        {
            var loans = mysqlService.GetLoans();
            var activeLoans = loans.Where(l => l.Status == "active" && l.Loaner_id == loanerId).ToList();
            List<ActiveLoansPreviewMongo> activeLoansPreviewMongos = new List<ActiveLoansPreviewMongo>();
            foreach (var activeLoan in activeLoans)
            {
                ActiveLoansPreviewMongo activeLoanPreviewMongo = new ActiveLoansPreviewMongo
                {
                    LoanId = ObjectId.GenerateNewId(),
                    Item_Name = mysqlService.GetItems().FirstOrDefault(i => i.Id == mysqlService.GetInventories().FirstOrDefault(inv => inv.Id == activeLoan.Inventory_id)?.Item_id)?.Name ?? "",
                    Due_Date = activeLoan.Due_date
                };
                activeLoansPreviewMongos.Add(activeLoanPreviewMongo);
            }
            return activeLoansPreviewMongos;
        }
        // Transforms active reservations from MySQL to MongoDB format, where id is the loaner id, used in TransformLoaners
        public List<ActiveReservationsPreviewMongo> TransformActiveReservationsPreview(int loanerId)
        {
            var reservations = mysqlService.GetReservations();
            var activeReservations = reservations.Where(r => r.Status == "active" && r.Loaner_id == loanerId).ToList();
            List<ActiveReservationsPreviewMongo> activeReservationsPreviewMongos = new List<ActiveReservationsPreviewMongo>();
            foreach (var activeReservation in activeReservations)
            {
                ActiveReservationsPreviewMongo activeReservationPreviewMongo = new ActiveReservationsPreviewMongo
                {
                    ReservationId = ObjectId.GenerateNewId(),
                    ItemId = activeReservation.Item_id,
                    ItemName = mysqlService.GetItems().FirstOrDefault(i => i.Id == activeReservation.Item_id)?.Name ?? "",
                    QueueNumber = activeReservation.Queue_number,
                    Status = activeReservation.Status
                };
                activeReservationsPreviewMongos.Add(activeReservationPreviewMongo);
            }
            return activeReservationsPreviewMongos;
        }
        // Transforms Loaners from MySQL to MongoDB format
        public List<LoanersMongo> TransformLoaners()
        {
            // Filters active reservations to be used in the LoanersMongo transformation
            var reservations = mysqlService.GetReservations();
            var activeReservations = reservations.Where(r => r.Status == "active").ToList();

            // Turns MySQL Loaner into MongoDB LoanerMongo
            var loaners = mysqlService.GetLoaners();
            List<LoanersMongo> loanerMongos = new List<LoanersMongo>();
            foreach (Loaner loaner in loaners)
            {
                LoanersMongo loanerMongo = new LoanersMongo
                {
                    Id = ObjectId.GenerateNewId(),
                    FirstName = loaner.First_name,
                    LastName = loaner.Last_name,
                    Email = loaner.Email,
                    Tlf = loaner.Tlf,
                    Cpr = loaner.CPR,
                    PasswordHash = loaner.Password,

                    ActiveLoans = TransformActiveLoansPreview(loaner.Id),
                    ActiveReservations =TransformActiveReservationsPreview(loaner.Id)

                };
                loanerMongos.Add(loanerMongo);
            }
            return loanerMongos;
        }
        // Transform Fines from MySQL to MongoDB, used in TransformLoans
        public List<FineMongo> TransformFines(int loanId)
        {
            var fines = mysqlService.GetFines().Where(f => f.Loan_id == loanId).ToList();
            List<FineMongo> fineMongos = new List<FineMongo>();
            foreach (var fine in fines)
            {
                FineMongo fineMongo = new FineMongo
                {
                    Id = ObjectId.GenerateNewId(),
                    Amount = fine.Amount,
                    Status = fine.Status,
                    CreatedDate = fine.Created_date,
                    DueDate = fine.Due_date,
                    PaidDate = fine.Paid_date ?? DateTime.MinValue
                };
                fineMongos.Add(fineMongo);
            }
            return fineMongos;
        }
        // Transforms Loans from MySQL to MongoDB
        public List<LoansMongo> TransformLoans()
        {
            var loans = mysqlService.GetLoans();
            List<LoansMongo> loansMongos = new List<LoansMongo>();
            foreach (var loan in loans)
            {
                LoansMongo loanMongo = new LoansMongo
                {
                    Id = ObjectId.GenerateNewId(),
                    Loaner_Id = loan.Loaner_id,
                    InventoryId = loan.Inventory_id,
                    Loan_Date = loan.Loan_date,
                    Due_Date = loan.Due_date,
                    Return_Date = loan.Return_date,
                    Status = loan.Status,

                    Item_Snapshot = new ItemSnapshot
                    {
                        Name = mysqlService.GetItems().FirstOrDefault(i => i.Id == mysqlService.GetInventories().FirstOrDefault(inv => inv.Id == loan.Inventory_id)?.Item_id)?.Name ?? "",
                        MediaType = mysqlService.GetItems().FirstOrDefault(i => i.Id == mysqlService.GetInventories().FirstOrDefault(inv => inv.Id == loan.Inventory_id)?.Item_id)?.Media_type ?? ""
                    },
                    Inventory_Snapshot = new InventorySnapshot
                    {
                        Barcode = mysqlService.GetInventories().FirstOrDefault(inv => inv.Id == loan.Inventory_id)?.Barcode ?? "",
                    },
                    Fines = TransformFines(loan.Id)
                };
                loansMongos.Add(loanMongo); 
            }
            return loansMongos;
        }
        // Transforms Reservations from MySQL to MongoDB
        public List<ReservationsMongo> TransformReservations()
        {
            var reservations = mysqlService.GetReservations();
            List<ReservationsMongo> reservationsMongos = new List<ReservationsMongo>();
            foreach (var reservation in reservations)
            {
                ReservationsMongo reservationMongo = new ReservationsMongo
                {
                    Id = ObjectId.GenerateNewId(),
                    Loaner_Id = reservation.Loaner_id,
                    Item_Id = reservation.Item_id,
                    Item_Name = mysqlService.GetItems().FirstOrDefault(i => i.Id == reservation.Item_id)?.Name ?? "",
                    Created_At = DateTime.Now, // We have no prior info, so we set current time instead of null
                    Status = reservation.Status
                };
                reservationsMongos.Add(reservationMongo);
            }
            return reservationsMongos;
        }

        // Clear existing data in MongoDB collection
        public async Task ClearCollection(string collectionName)
        {
            try
            {
                var database = _client.GetDatabase(_databaseName);
                var collection = database.GetCollection<dynamic>(collectionName);
                collection.DeleteMany(Builders<dynamic>.Filter.Empty);
                Console.WriteLine($"Collection '{collectionName}' cleared successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to clear collection '{collectionName}': {ex.Message}");
            }
        }

        // Insert Data into MongoDB
        public async Task InsertData<T>(string collectionName, List<T> data)
        {
            try
            {
                var collection = _database.GetCollection<T>(collectionName);
                collection.InsertMany(data);
                Console.WriteLine($"Data inserted into MongoDB collection '{collectionName}' successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to insert data into MongoDB: {ex.Message}");
            }
        }
    }
}
