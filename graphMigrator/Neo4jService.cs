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
            await session.ExecuteWriteAsync(async wa =>
            {
                await wa.RunAsync(query);
            });
        }

        public async Task CreateLoaner(List<Loaner> loaners)
        {
            var query = @" 
            UNWIND $loaners AS l
            MERGE (lo:loaner {id: l.Id})
            SET lo.first_name = l.First_name, lo.last_name = l.Last_name, lo.cpr = l.CPR, lo.tlf = l.Tlf, lo.email = l.Email, lo.password = l.Password";

            await using var session = _driver.AsyncSession(o => o.WithDatabase(_database));
            await session.ExecuteWriteAsync(async wa =>
            {
                await wa.RunAsync(query, new { loaners });
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
            await session.ExecuteWriteAsync(async wa =>
            {
                await wa.RunAsync(query, new { items });
            });
        }
        public async Task CreateCreator(List<Creator> creators)
        {
            var query = @" 
            UNWIND $creators AS c
            MERGE (cr:creator {id: c.Id})
            SET cr.first_name = c.First_name, cr.last_name = c.Last_name, cr.birthday = cr.Birthday, cr.description = c.Description";
            await using var session = _driver.AsyncSession(o => o.WithDatabase(_database));
            await session.ExecuteWriteAsync(async wa =>
            {
                await wa.RunAsync(query, new { creators });
            });
        }
        public async Task CreatePublisher(List<Publisher> publishers)
        {
            var query = @"
            UNWIND $publishers AS p
            MERGE (pu:publisher {id: p.Id})
            SET pu.name = p.Name";
            await using var session = _driver.AsyncSession(o => o.WithDatabase(_database));
            await session.ExecuteWriteAsync(async wa =>
            {
                await wa.RunAsync(query, new { publishers });
            });
        }
        public async Task CreateBook(List<Book> books)
        {
            var query = @" 
            UNWIND $books AS b
            MERGE (bo:book {id: b.Id})
            SET bo.ISBN = b.ISBN, bo.no_of_pages = b.No_of_pages, bo.version = b.Version, bo.item_id = b.Item_id";
            await using var session = _driver.AsyncSession(o => o.WithDatabase(_database));
            await session.ExecuteWriteAsync(async wa =>
            {
                await wa.RunAsync(query, new { books});
            });
        }
    }
}
