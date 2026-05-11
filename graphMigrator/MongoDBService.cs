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
            FetchData();
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

        // MySQL Models
        List<BoardGame> boardgameDetails;
        List<Book> books;
        List<Creator> creators;
        List<Fine> fines;
        List<Genre> genres;
        List<Inventory> inventories;
        List<Item> mySqlItems;
        List<ItemCreator> itemCreators;
        List<ItemGenre> itemGenres;
        List<ItemTag> itemTags;
        List<Language> languages;
        List<Loan> loans;
        List<Loaner> loaners;
        List<Publisher> publishers;
        List<Reservation> reservations;
        List<Review> reviews;
        List<Tag> tags;

        private void FetchData()
        {
            boardgameDetails = mysqlService.GetBoardGames();
            books = mysqlService.GetBooks();
            creators = mysqlService.GetCreators();
            fines = mysqlService.GetFines();
            genres = mysqlService.GetGenres();
            inventories = mysqlService.GetInventories();
            mySqlItems = mysqlService.GetItems();
            itemCreators = mysqlService.GetItemCreators();
            itemGenres = mysqlService.GetItemGenres();
            itemTags = mysqlService.GetItemTags();
            languages = mysqlService.GetLanguages();
            loans = mysqlService.GetLoans();
            loaners = mysqlService.GetLoaners();
            publishers = mysqlService.GetPublishers();
            reservations = mysqlService.GetReservations();
            reviews = mysqlService.GetReviews();
            tags = mysqlService.GetTags();
        }

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
            // Turns MySQL Genre into MongoDB GenreMongo
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
            // Turns MySQL Tag into MongoDB TagMongo
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
            // Turns MySQL BookDetails into MongoDB 
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
            List<BoardgameDetailsMongo> boardgameDetailsMongos = new List<BoardgameDetailsMongo>();
            foreach (BoardGame boardgameDetail in boardgameDetails)
            {
                BoardgameDetailsMongo boardgameDetailsMongo = new BoardgameDetailsMongo
                {
                    No_Of_Players = boardgameDetail.No_of_players,
                    Play_Time = boardgameDetail.Play_time,
                    Age_Group = boardgameDetail.Age_group,
                    item_id = boardgameDetail.Item_id
                };
                boardgameDetailsMongos.Add(boardgameDetailsMongo);
              
            }

            // Turns MySQL Review into MongoDB ReviewMongo and also adds the Loaner name to the ReviewMongo
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
            List<ItemMongo> itemMongos = new List<ItemMongo>();
            foreach (Item item in mySqlItems)
            {
                ItemMongo itemMongo = new ItemMongo
                {
                    _id = ObjectId.GenerateNewId(),
                    Id = item.Id, // This is the Id from the SQL database, not the MongoDB _id
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
            List<InventoryMongo> inventoryMongos = new List<InventoryMongo>();
            foreach (Inventory inventory in inventories)
            {
                InventoryMongo inventoryMongo = new InventoryMongo
                {
                    _id = ObjectId.GenerateNewId(),
                    Id = inventory.Id, // This is the Id from the SQL database, not the MongoDB _id
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
            var activeLoans = loans.Where(l => l.Status == "active" && l.Loaner_id == loanerId).ToList();
            List<ActiveLoansPreviewMongo> activeLoansPreviewMongos = new List<ActiveLoansPreviewMongo>();
            foreach (var activeLoan in activeLoans)
            {
                ActiveLoansPreviewMongo activeLoanPreviewMongo = new ActiveLoansPreviewMongo
                {
                    LoanId = ObjectId.GenerateNewId(),
                    Item_Name = mySqlItems.FirstOrDefault(i => i.Id == inventories.FirstOrDefault(inv => inv.Id == activeLoan.Inventory_id)?.Item_id)?.Name ?? "",
                    Due_Date = activeLoan.Due_date
                };
                activeLoansPreviewMongos.Add(activeLoanPreviewMongo);
            }
            return activeLoansPreviewMongos;
        }
        // Transforms active reservations from MySQL to MongoDB format, where id is the loaner id, used in TransformLoaners
        public List<ActiveReservationsPreviewMongo> TransformActiveReservationsPreview(int loanerId)
        {
            var activeReservations = reservations.Where(r => r.Status == "active" && r.Loaner_id == loanerId).ToList();
            List<ActiveReservationsPreviewMongo> activeReservationsPreviewMongos = new List<ActiveReservationsPreviewMongo>();
            foreach (var activeReservation in activeReservations)
            {
                ActiveReservationsPreviewMongo activeReservationPreviewMongo = new ActiveReservationsPreviewMongo
                {
                    ReservationId = ObjectId.GenerateNewId(),
                    ItemId = activeReservation.Item_id,
                    ItemName = mySqlItems.FirstOrDefault(i => i.Id == activeReservation.Item_id)?.Name ?? "",
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
            var activeReservations = reservations.Where(r => r.Status == "active").ToList();

            // Turns MySQL Loaner into MongoDB LoanerMongo
            List<LoanersMongo> loanerMongos = new List<LoanersMongo>();
            foreach (Loaner loaner in loaners)
            {
                LoanersMongo loanerMongo = new LoanersMongo
                {
                    _id = ObjectId.GenerateNewId(),
                    Id = loaner.Id, // This is the Id from the SQL database, not the MongoDB _id
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
            var mySqlFines = fines.Where(f => f.Loan_id == loanId).ToList();
            List<FineMongo> fineMongos = new List<FineMongo>();
            foreach (var fine in mySqlFines)
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
            List<LoansMongo> loansMongos = new List<LoansMongo>();
            foreach (var loan in loans)
            {
                LoansMongo loanMongo = new LoansMongo
                {
                    _id = ObjectId.GenerateNewId(),
                    Id = loan.Id, // This is the Id from the SQL database, not the MongoDB _id
                    Loaner_Id = loan.Loaner_id,
                    InventoryId = loan.Inventory_id,
                    Loan_Date = loan.Loan_date,
                    Due_Date = loan.Due_date,
                    Return_Date = loan.Return_date,
                    Status = loan.Status,

                    Item_Snapshot = new ItemSnapshot
                    {
                        Name = mySqlItems.FirstOrDefault(i => i.Id == inventories.FirstOrDefault(inv => inv.Id == loan.Inventory_id)?.Item_id)?.Name ?? "",
                        MediaType = mySqlItems.FirstOrDefault(i => i.Id == inventories.FirstOrDefault(inv => inv.Id == loan.Inventory_id)?.Item_id)?.Media_type ?? ""
                    },
                    Inventory_Snapshot = new InventorySnapshot
                    {
                        Barcode = inventories.FirstOrDefault(inv => inv.Id == loan.Inventory_id)?.Barcode ?? "",
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
            List<ReservationsMongo> reservationsMongos = new List<ReservationsMongo>();
            foreach (var reservation in reservations)
            {
                ReservationsMongo reservationMongo = new ReservationsMongo
                {
                    _id = ObjectId.GenerateNewId(),
                    Id = reservation.Id, // This is the Id from the SQL database, not the MongoDB _id
                    Loaner_Id = reservation.Loaner_id,
                    Item_Id = reservation.Item_id,
                    Item_Name = mySqlItems.FirstOrDefault(i => i.Id == reservation.Item_id)?.Name ?? "",
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
