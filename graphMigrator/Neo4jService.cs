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

        public async Task Neo4jTransaction<T>(IAsyncTransaction transaction, List<T> objects, string query)
        {
            await transaction.RunAsync(query, new { objects });
        }
        public Dictionary<string, string> nodeQueries = new Dictionary<string, string>
        {
            {
                "Loaner",
                @"UNWIND $objects AS l
                MERGE (lo:loaner {id: l.Id})
                SET lo.first_name = l.First_name, lo.last_name = l.Last_name, lo.cpr = l.CPR, lo.tlf = l.Tlf, lo.email = l.Email, lo.password = l.Password;"
            },
            {
                "Language",
                @"UNWIND $objects AS l
                MERGE (la:language {id: l.Id})
                SET la.name = l.Name"
            },
            {
                "Item",
                @"UNWIND $objects AS i
                MERGE (it:item {id: i.Id})
                SET it.name = i.Name, it.release_year = i.Release_year, it.description = i.Description, it.review_summary = i.Review_summary, 
                it.media_type = i.Media_type, it.image = i.Image, it.average_stars = i.Average_stars"
            },
            {
                "Creator",
                @"UNWIND $objects AS c
                MERGE (cr:creator {id: c.Id})
                SET cr.first_name = c.First_name, cr.last_name = c.Last_name, cr.birthday = cr.Birthday, cr.description = c.Description"
            },
            {
                "Publisher",
                @"UNWIND $objects AS p
                MERGE (pu:publisher {id: p.Id})
                SET pu.name = p.Name"
            },
            {
                "Book",
                @"UNWIND $objects AS b
                MERGE (bo:book {id: b.Id})
                SET bo.ISBN = b.ISBN, bo.no_of_pages = b.No_of_pages, bo.version = b.Version"
            },
            {
                "Genre",
                @"UNWIND $objects AS g
                MERGE (ge:genre {id: g.Id})
                SET ge.name = g.Name"
            },
            {
                "Tag",
                @"UNWIND $objects AS t
                MERGE (ta:tag {id: t.Id})
                SET ta.name = t.Name"
            },
            {
                "Inventory",
                @"UNWIND $objects AS i
                MERGE (in:inventory {id: i.Id})
                SET in.status = i.Status, in.barcode = i.Barcode, in.placement = i.Placement"
            },
            {
                "Loan",
                @"UNWIND $objects AS l
                MERGE (lo:loan {id: l.Id})
                SET lo.loan_date = l.Loan_date, lo.due_date = l.Due_date, lo.return_date = l.Return_date, lo.status = l.Status"
            },
            {
                "Reservation",
                @"UNWIND $objects AS r
                MERGE (re:reservation {id: r.Id})
                SET re.status = r.Status, re.queue_number = r. Queue_number"
            },
            {
                "Fine",
                @"UNWIND $objects AS f
                MERGE (fi:fine {id: f.Id})
                SET fi.amount = f.Amount, fi.status = f.Status, fi.created_date = f.Created_date, fi.paid_date = f.Paid_date, fi.due_date = f.Due_date"
            },
            {
                "Boardgame",
                @"UNWIND $objects AS b
                MERGE (bg:boardgame {id: b.Id})
                SET bg.no_of_players = b.No_of_players, bg.play_time = b.Play_time, bg.age_group = b.Age_group"
            },
            {
                "Item_Language",
                @"UNWIND $objects AS i
                MATCH (it:item {id: i.Id}), (la:language {id: i.Language_id})
                MERGE (it)-[:HAS_LANGUAGE]->(la)"
            },
            {
                "Item_Publisher",
                @"UNWIND $objects AS i
                MATCH (it:item {id: i.Id}), (pu:publisher {id: i.Publisher_id})
                MERGE (it)-[:PUBLISHED_BY]->(pu)"
            },
            {
                "Item_Creator",
                @"UNWIND $objects AS ic
                MATCH (it:item {id: ic.Item_id}), (cr:creator {id: ic.Creator_id})
                MERGE (it)-[:CREATED_BY]->(cr)"
            },
            {
                "Book_Item",
                @"UNWIND $objects AS b
                MATCH (bo:book {id: b.Id}), (it:item {id: b.Id})
                MERGE (bo)-[:IS_BOOK]->(it)"
            },
            {
                "Boardgame_Item",
                @"UNWIND $objects AS b
                MATCH (bg:boardgame {id: b.Id}), (it:item {id: b.Item_id})
                MERGE (bg)-[:IS_BOARDGAME]->(it)"
            },
            {
                "Item_Genre",
                @"UNWIND $objects AS ig
                MATCH (it:item {id: ig.Item_id}), (ge:genre {id: ig.Genre_id})
                MERGE (it)-[:GENRE_IS]->(ge)"
            },
            {
                "Item_Tag",
                @"UNWIND $objects AS it
                MATCH (i:item {id: it.Item_id}), (t:tag {id: it.Tag_id})
                MERGE (i)-[:TAGGED_AS]->(t)"
            },
            {
                "Review",
                @"UNWIND $objects AS r
                CREATE (re:review)
                SET re.no_of_stars = r.No_of_stars, re.text = r.Text
                WITH re, r
                MATCH (it:item {id: r.Item_id})
                MERGE (re)-[:REVIEW_FOR]->(it)
                WITH re, r
                MATCH (lo:loaner {id: r.Loaner_id})
                MERGE (re)-[:REVIEW_BY]->(lo)"
            },
            {
                "Item_Reservation",
                @"UNWIND $objects AS r
                MATCH (re:reservation {id: r.Id}), (it:item {id: r.Item_id})
                MERGE (re)-[:RESERVE_ITEM]->(it)"
            },
            {
                "Item_Inventory",
                @"UNWIND $objects AS i
                MATCH (in:inventory {id: i.Id}), (it:item {id: i.Item_id})
                MERGE (in)-[:STORES_ITEM]->(it)"
            },
            {
                "Loaner_Reservation",
                @"UNWIND $objects AS r
                MATCH (re:reservation {id: r.Id}), (lo:loaner {id: r.Loaner_id})
                MERGE (lo)-[:MADE_RESERVATION]->(re)"
            },
            {
                "Loaner_Loan",
                @"UNWIND $objects AS l
                MATCH (lo:loan {id: l.Id}), (loaner:loaner {id: l.Loaner_id})
                MERGE (loaner)-[:MADE_LOAN]->(lo)"
            },
            {
                "Fine_Loan",
                @"UNWIND $objects AS f
                MATCH (fi:fine {id: f.Id}), (lo:loan {id: f.Loan_id})
                MERGE (lo)-[:HAS_FINE]->(fi)"
            },
            {
                "Loan_Inventory",
                @"UNWIND $objects AS l
                MATCH (lo:loan {id: l.Id}), (in:inventory {id: l.Inventory_id})
                MERGE (lo)-[:LOANS_FROM]->(in)"
            }
        };
    }
}
