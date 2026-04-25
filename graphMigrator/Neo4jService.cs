using graphMigrator.Models;
using Neo4j.Driver;

namespace graphMigrator
{
    public class Neo4jService
    {
        private readonly IDriver _driver;
        private readonly string _database;

        public Neo4jService(string uri, string user, string password, string database)
        {
            _driver = GraphDatabase.Driver(uri, AuthTokens.Basic(user, password));
            _database = database;
        }

        public async Task ExecuteInTransaction(Func<IAsyncTransaction, Task> action)
        {
            await using var session = _driver.AsyncSession(o => o.WithDatabase(_database));
            await using var transaction = await session.BeginTransactionAsync();

            try
            {
                await action(transaction);
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task DeleteEverything(IAsyncTransaction transaction)
        {
            var query = @"MATCH (n) DETACH DELETE n";

            await transaction.RunAsync(query);
        }

        public async Task CreateNode<T>(IAsyncTransaction transaction, List<T> objects, string query)
        {
            await transaction.RunAsync(query, new { objects });
        }
        public async Task CreateLoaner(IAsyncTransaction transaction, List<Loaner> loaners)
        {
            var query = @" 
            UNWIND $loaners AS l
            MERGE (lo:loaner {id: l.Id})
            SET lo.first_name = l.First_name, lo.last_name = l.Last_name, lo.cpr = l.CPR, lo.tlf = l.Tlf, lo.email = l.Email, lo.password = l.Password";

            await transaction.RunAsync(query, new { loaners });
        }
        public async Task CreateLanguage(IAsyncTransaction transaction, List<Language> languages)
        {
            var query = @"
            UNWIND $languages AS l
            MERGE (la:language {id: l.Id})
            SET la.name = l.Name";

            await transaction.RunAsync(query, new { languages });
        }
        public async Task CreateItem(IAsyncTransaction transaction, List<Item> items)
        {
            var query = @" 
            UNWIND $items AS i
            MERGE (it:item {id: i.Id})
            SET it.name = i.Name, it.release_year = i.Release_year, it.description = i.Description, it.review_summary = i.Review_summary, 
            it.media_type = i.Media_type, it.image = i.Image, it.average_stars = i.Average_stars";
            
            await transaction.RunAsync(query, new { items });
        }
        public async Task CreateCreator(IAsyncTransaction transaction, List<Creator> creators)
        {
            var query = @" 
            UNWIND $creators AS c
            MERGE (cr:creator {id: c.Id})
            SET cr.first_name = c.First_name, cr.last_name = c.Last_name, cr.birthday = cr.Birthday, cr.description = c.Description";

            await transaction.RunAsync(query, new { creators });
        }
        public async Task CreatePublisher(IAsyncTransaction transaction, List<Publisher> publishers)
        {
            var query = @"
            UNWIND $publishers AS p
            MERGE (pu:publisher {id: p.Id})
            SET pu.name = p.Name";

            await transaction.RunAsync(query, new { publishers });
        }
        public async Task CreateBook(IAsyncTransaction transaction, List<Book> books)
        {
            var query = @" 
            UNWIND $books AS b
            MERGE (bo:book {id: b.Id})
            SET bo.ISBN = b.ISBN, bo.no_of_pages = b.No_of_pages, bo.version = b.Version";

            await transaction.RunAsync(query, new { books });

        }
        public async Task CreateGenre(IAsyncTransaction transaction, List<Genre> genres)
        {
            var query = @"
            UNWIND $genres AS g
            MERGE (ge:genre {id: g.Id})
            SET ge.name = g.Name";

            await transaction.RunAsync(query, new { genres });
        }
        public async Task CreateTag(IAsyncTransaction transaction, List<Tag> tags)
        {
            var query = @"
            UNWIND $tags AS t
            MERGE (ta:tag {id: t.Id})
            SET ta.name = t.Name";

            await transaction.RunAsync(query, new { tags });
        }
        public async Task CreateInventory(IAsyncTransaction transaction, List<Inventory> inventories)
        {
            var query = @"
            UNWIND $inventories AS i
            MERGE (in:inventory {id: i.Id})
            SET in.status = i.Status, in.barcode = i.Barcode, in.placement = i.Placement";

            await transaction.RunAsync(query, new { inventories });
        }
        public async Task CreateLoan(IAsyncTransaction transaction, List<Loan> loans)
        {
            var query = @"
            UNWIND $loans AS l
            MERGE (lo:loan {id: l.Id})
            SET lo.loan_date = l.Loan_date, lo.due_date = l.Due_date, lo.return_date = l.Return_date, lo.status = l.Status";

            await transaction.RunAsync(query, new { loans });
        }
        public async Task CreateReservation(IAsyncTransaction transaction, List<Reservation> reservations)
        {
            var query = @"
            UNWIND $reservations AS r
            MERGE (re:reservation {id: r.Id})
            SET re.status = r.Status, re.queue_number = r. Queue_number";

            await transaction.RunAsync(query, new { reservations });
        }
        public async Task CreateFine(IAsyncTransaction transaction, List<Fine> fines)
        {
            var query = @"
            UNWIND $fines AS f
            MERGE (fi:fine {id: f.Id})
            SET fi.amount = f.Amount, fi.status = f.Status, fi.created_date = f.Created_date, fi.paid_date = f.Paid_date, fi.due_date = f.Due_date";

            await transaction.RunAsync(query, new { fines });
        }
        public async Task CreateBoardGame(IAsyncTransaction transaction, List<BoardGame> boardgames)
        {
            var query = @"
            UNWIND $boardgames AS b
            MERGE (bg:boardgame {id: b.Id})
            SET bg.no_of_players = b.No_of_players, bg.play_time = b.Play_time, bg.age_group = b.Age_group";

            await transaction.RunAsync(query, new { boardgames });
        }
        public async Task Item_Language(IAsyncTransaction transaction, List<Item> items)
        {
            var query = @"
            UNWIND $items AS i
            MATCH (it:item {id: i.Id}), (la:language {id: i.Language_id})
            MERGE (it)-[:HAS_LANGUAGE]->(la)";

            await transaction.RunAsync(query, new { items });
        }
        public async Task Item_Publisher(IAsyncTransaction transaction, List<Item> items)
        {
            var query = @"
            UNWIND $items AS i
            MATCH (it:item {id: i.Id}), (pu:publisher {id: i.Publisher_id})
            MERGE (it)-[:PUBLISHED_BY]->(pu)";

            await transaction.RunAsync(query, new { items });
        }
        public async Task Item_Creator(IAsyncTransaction transaction, List<ItemCreator> itemCreators)
        {
            var query = @"
            UNWIND $itemCreators AS ic
            MATCH (it:item {id: ic.Item_id}), (cr:creator {id: ic.Creator_id})
            MERGE (it)-[:CREATED_BY]->(cr)";

            await transaction.RunAsync(query, new { itemCreators });
        }
        public async Task Book_item(IAsyncTransaction transaction, List<Book> books)
        {
            var query = @"
            UNWIND $books AS b
            MATCH (bo:book {id: b.Id}), (it:item {id: b.Id})
            MERGE (bo)-[:IS_BOOK]->(it)";

            await transaction.RunAsync(query, new { books });
        }
        public async Task Boardgame_item(IAsyncTransaction transaction, List<BoardGame> boardgames)
        {
            var query = @"
            UNWIND $boardgames AS b
            MATCH (bg:boardgame {id: b.Id}), (it:item {id: b.Item_id})
            MERGE (bg)-[:IS_BOARDGAME]->(it)";

            await transaction.RunAsync(query, new { boardgames });
        }
        public async Task Item_Genre(IAsyncTransaction transaction, List<ItemGenre> itemGenres)
        {
            var query = @"
            UNWIND $itemGenres AS ig
            MATCH (it:item {id: ig.Item_id}), (ge:genre {id: ig.Genre_id})
            MERGE (it)-[:GENRE_IS]->(ge)";

            await transaction.RunAsync(query, new { itemGenres });
        }
        public async Task Item_Tag(IAsyncTransaction transaction, List<ItemTag> itemTags)
        {
            var query = @"
            UNWIND $itemTags AS it
            MATCH (i:item {id: it.Item_id}), (t:tag {id: it.Tag_id})
            MERGE (i)-[:TAGGED_AS]->(t)";

            await transaction.RunAsync(query, new { itemTags });
        }
        public async Task CreateReview(IAsyncTransaction transaction, List<Review> reviews)
        {
            var query = @"
            UNWIND $reviews AS r
            CREATE (re:review)
            SET re.no_of_stars = r.No_of_stars, re.text = r.Text
            WITH re, r
            MATCH (it:item {id: r.Item_id})
            MERGE (re)-[:REVIEW_FOR]->(it)
            WITH re, r
            MATCH (lo:loaner {id: r.Loaner_id})
            MERGE (re)-[:REVIEW_BY]->(lo)";

            await transaction.RunAsync(query, new { reviews });
        }
        public async Task Item_Reservation(IAsyncTransaction transaction, List<Reservation> reservations)
        {
            var query = @"
            UNWIND $reservations AS r
            MATCH (re:reservation {id: r.Id}), (it:item {id: r.Item_id})
            MERGE (re)-[:RESERVE_ITEM]->(it)";

            await transaction.RunAsync(query, new { reservations });
        }
        public async Task Item_Inventory(IAsyncTransaction transaction, List<Inventory> inventories)
        {
            var query = @"
            UNWIND $inventories AS i
            MATCH (in:inventory {id: i.Id}), (it:item {id: i.Item_id})
            MERGE (in)-[:STORES_ITEM]->(it)";

            await transaction.RunAsync(query, new { inventories });
        }
        public async Task Loaner_Reservation(IAsyncTransaction transaction, List<Reservation> reservations)
        {
            var query = @"
            UNWIND $reservations AS r
            MATCH (re:reservation {id: r.Id}), (lo:loaner {id: r.Loaner_id})
            MERGE (lo)-[:MADE_RESERVATION]->(re)";

            await transaction.RunAsync(query, new { reservations });
        }
        public async Task Loaner_Loan(IAsyncTransaction transaction, List<Loan> loans)
        {
            var query = @"
            UNWIND $loans AS l
            MATCH (lo:loan {id: l.Id}), (loaner:loaner {id: l.Loaner_id})
            MERGE (loaner)-[:MADE_LOAN]->(lo)";

            await transaction.RunAsync(query, new { loans });
        }
        public async Task Loan_Fine(IAsyncTransaction transaction, List<Fine> fines)
        {
            var query = @"
            UNWIND $fines AS f
            MATCH (fi:fine {id: f.Id}), (lo:loan {id: f.Loan_id})
            MERGE (lo)-[:HAS_FINE]->(fi)";

            await transaction.RunAsync(query, new { fines });
        }
        public async Task Loan_Inventory(IAsyncTransaction transaction, List<Loan> loans)
        {
            var query = @"
            UNWIND $loans AS l
            MATCH (lo:loan {id: l.Id}), (in:inventory {id: l.Inventory_id})
            MERGE (lo)-[:LOANS_FROM]->(in)";

            await transaction.RunAsync(query, new { loans });
        }
    }
}
