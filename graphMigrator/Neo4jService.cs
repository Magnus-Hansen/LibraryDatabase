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
        public async Task DeleteEverything()
        {
            var query = @"MATCH (n) DETACH DELETE n";
            await using var session = _driver.AsyncSession(o => o.WithDatabase(_database));
            await session.ExecuteWriteAsync(async tx =>
            {
                await tx.RunAsync(query);
            });
        }

        public async Task CreateLoaner(List<Loaner> loaners)
        {
            var query = @" 
            UNWIND $loaners AS l
            MERGE (lo:loaner {id: l.Id})
            SET lo.first_name = l.First_name, lo.last_name = l.Last_name, lo.cpr = l.CPR, lo.tlf = l.Tlf, lo.email = l.Email, lo.password = l.Password";

            await using var session = _driver.AsyncSession(o => o.WithDatabase(_database));
            await session.ExecuteWriteAsync(async tx =>
            {
                await tx.RunAsync(query, new { loaners });
            });
        }
        public async Task CreateLanguage(List<Language> languages)
        {
            var query = @"
            UNWIND $languages AS l
            MERGE (la:language {id: l.Id})
            SET la.name = l.Name";
            await using var session = _driver.AsyncSession(o => o.WithDatabase(_database));

            try
            {
                await session.ExecuteWriteAsync(async tx =>
                {
                    await tx.RunAsync(query, new { languages });
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating languages: {ex.Message}");
            }
        }
        public async Task CreateItem(List<Item> items)
        {
            var query = @" 
            UNWIND $items AS i
            MERGE (it:item {id: i.Id})
            SET it.name = i.Name, it.release_year = i.Release_year, it.description = i.Description, it.review_summary = i.Review_summary, 
            it.media_type = i.Media_type, it.image = i.Image, it.average_stars = i.Average_stars";
            await using var session = _driver.AsyncSession(o => o.WithDatabase(_database));
            await session.ExecuteWriteAsync(async tx =>
            {
                await tx.RunAsync(query, new { items });
            });
        }
        public async Task CreateCreator(List<Creator> creators)
        {
            var query = @" 
            UNWIND $creators AS c
            MERGE (cr:creator {id: c.Id})
            SET cr.first_name = c.First_name, cr.last_name = c.Last_name, cr.birthday = cr.Birthday, cr.description = c.Description";
            await using var session = _driver.AsyncSession(o => o.WithDatabase(_database));
            await session.ExecuteWriteAsync(async tx =>
            {
                await tx.RunAsync(query, new { creators });
            });
        }
        public async Task CreatePublisher(List<Publisher> publishers)
        {
            var query = @"
            UNWIND $publishers AS p
            MERGE (pu:publisher {id: p.Id})
            SET pu.name = p.Name";
            await using var session = _driver.AsyncSession(o => o.WithDatabase(_database));
            await session.ExecuteWriteAsync(async tx =>
            {
                await tx.RunAsync(query, new { publishers });
            });
        }
        public async Task CreateBook(List<Book> books)
        {
            var query = @" 
            UNWIND $books AS b
            MERGE (bo:book {id: b.Id})
            SET bo.ISBN = b.ISBN, bo.no_of_pages = b.No_of_pages, bo.version = b.Version";
            await using var session = _driver.AsyncSession(o => o.WithDatabase(_database));
            await session.ExecuteWriteAsync(async tx =>
            {
                await tx.RunAsync(query, new { books });
            });
        }
        public async Task CreateGenre(List<Genre> genres)
        {
            var query = @"
            UNWIND $genres AS g
            MERGE (ge:genre {id: g.Id})
            SET ge.name = g.Name";
            await using var session = _driver.AsyncSession(o => o.WithDatabase(_database));
            await session.ExecuteWriteAsync(async tx =>
            {
                await tx.RunAsync(query, new { genres });
            });
        }
        public async Task CreateTag(List<Tag> tags)
        {
            var query = @"
            UNWIND $tags AS t
            MERGE (ta:tag {id: t.Id})
            SET ta.name = t.Name";
            await using var session = _driver.AsyncSession(o => o.WithDatabase(_database));
            await session.ExecuteWriteAsync(async tx =>
            {
                await tx.RunAsync(query, new { tags });
            });
        }
        public async Task CreateInventory(List<Inventory> inventories)
        {
            var query = @"
            UNWIND $inventories AS i
            MERGE (in:inventory {id: i.Id})
            SET in.status = i.Status, in.barcode = i.Barcode, in.placement = i.Placement";
            await using var session = _driver.AsyncSession(o => o.WithDatabase(_database));
            await session.ExecuteWriteAsync(async tx =>
            {
                await tx.RunAsync(query, new { inventories });
            });
        }
        public async Task CreateLoan(List<Loan> loans)
        {
            var query = @"
            UNWIND $loans AS l
            MERGE (lo:loan {id: l.Id})
            SET lo.loan_date = l.Loan_date, lo.due_date = l.Due_date, lo.return_date = l.Return_date, lo.status = l.Status";
            await using var session = _driver.AsyncSession(o => o.WithDatabase(_database));
            await session.ExecuteWriteAsync(async tx =>
            {
                await tx.RunAsync(query, new { loans });
            });
        }
        public async Task CreateReservation(List<Reservation> reservations)
        {
            var query = @"
            UNWIND $reservations AS r
            MERGE (re:reservation {id: r.Id})
            SET re.status = r.Status, re.queue_number = r. Queue_number";
            await using var session = _driver.AsyncSession(o => o.WithDatabase(_database));
            await session.ExecuteWriteAsync(async tx =>
            {
                await tx.RunAsync(query, new { reservations });
            });
        }
        public async Task CreateReview(List<Review> reviews)
        {
            var query = @"
            UNWIND $reviews AS r
            MERGE (re:review)
            SET re.no_of_stars = r.No_of_stars, re.text = r.Text";
            await using var session = _driver.AsyncSession(o => o.WithDatabase(_database));
            await session.ExecuteWriteAsync(async tx =>
            {
                await tx.RunAsync(query, new { reviews });
            });
        }
        public async Task CreateFine(List<Fine> fines)
        {
            var query = @"
            UNWIND $fines AS f
            MERGE (fi:fine {id: f.Id})
            SET fi.amount = f.Amount, fi.status = f.Status, fi.created_date = f.Created_date, fi.paid_date = f.Paid_date, fi.due_date = f.Due_date";
            await using var session = _driver.AsyncSession(o => o.WithDatabase(_database));
            await session.ExecuteWriteAsync(async tx =>
            {
                await tx.RunAsync(query, new { fines });
            });
        }
        public async Task CreateBoardGame(List<BoardGame> boardgames)
        {
            var query = @"
            UNWIND $boardgames AS b
            MERGE (bg:boardgame {id: b.Id})
            SET bg.no_of_players = b.No_of_players, bg.play_time = b.Play_time, bg.age_group = b.Age_group";
            await using var session = _driver.AsyncSession(o => o.WithDatabase(_database));
            await session.ExecuteWriteAsync(async tx =>
            {
                await tx.RunAsync(query, new { boardgames });
            });
        }
        public async Task Item_Language(List<Item> items)
        {
            var query = @"
            UNWIND $items AS i
            MATCH (it:item {id: i.Id}), (la:language {id: i.Language_id})
            MERGE (it)-[:HAS_LANGUAGE]->(la)";
            await using var session = _driver.AsyncSession(o => o.WithDatabase(_database));
            await session.ExecuteWriteAsync(async tx =>
            {
                await tx.RunAsync(query, new { items });
            });
        }
        public async Task Item_Publisher(List<Item> items)
        {
            var query = @"
            UNWIND $items AS i
            MATCH (it:item {id: i.Id}), (pu:publisher {id: i.Publisher_id})
            MERGE (it)-[:PUBLISHED_BY]->(pu)";
            await using var session = _driver.AsyncSession(o => o.WithDatabase(_database));
            await session.ExecuteWriteAsync(async tx =>
            {
                await tx.RunAsync(query, new { items });
            });
        }
        public async Task Item_Creator(List<ItemCreator> itemCreators)
        {
            var query = @"
            UNWIND $itemCreators AS ic
            MATCH (it:item {id: ic.Item_id}), (cr:creator {id: ic.Creator_id})
            MERGE (it)-[:CREATED_BY]->(cr)";
            await using var session = _driver.AsyncSession(o => o.WithDatabase(_database));
            await session.ExecuteWriteAsync(async tx =>
            {
                await tx.RunAsync(query, new { itemCreators });
            });
        }
    }
}
