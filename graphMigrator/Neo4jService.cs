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

        public async Task CreateUser(Loaner loaner)
        {
            var query = @" 
            MERGE (lo:loaner {id: $id})
            SET lo.first_name = $first_name, lo.last_name = $last_name, lo.cpr = $cpr, lo.tlf = $tlf, lo.email = $email, lo.password = $password";

            await using var session = _driver.AsyncSession(o => o.WithDatabase(_database));
            await session.ExecuteWriteAsync(async wa =>
            {
                await wa.RunAsync(query, new
                {
                    id = loaner.Id,
                    first_name = loaner.FirstName,
                    last_name = loaner.LastName,
                    cpr = loaner.CPR,
                    tlf = loaner.Tlf,
                    email = loaner.Email,
                    password = loaner.Password
                });
            });
        }
        public async Task CreateLanguage(Language language)
        {
            var query = @" 
            MERGE (la:language {id: $id})
            SET la.name = $name";

            await using var session = _driver.AsyncSession(o => o.WithDatabase(_database));
            await session.ExecuteWriteAsync(async wa =>
            {
                await wa.RunAsync(query, new
                {
                    id = language.Id,
                    name = language.Name
                });
            });
        }
        public async Task CreateItem(Item item)
        {
            var query = @" 
            MERGE (i:item {id: $id})
            SET i.name = $name, i.release_year = $release_year, i.description = $description, i.review_summary = $review_summary, i.media_type = $media_type, i.image = $image, i.average_stars = $average_stars";
            await using var session = _driver.AsyncSession(o => o.WithDatabase(_database));
            await session.ExecuteWriteAsync(async wa =>
            {
                await wa.RunAsync(query, new
                {
                    id = item.Id,
                    name = item.Name,
                    release_year = item.Release_year,
                    description = item.Description,
                    review_summary = item.Review_summary,
                    media_type = item.Media_type,
                    image = item.Image,
                    language_id = item.Language_id,
                    publisher_id = item.Publisher_id,
                    average_stars = item.Average_stars
                });
            });
        }
        public async Task CreateCreator(Creator creator)
        {
            var query = @" 
            MERGE (c:creator {id: $id})
            SET c.first_name = $first_name, c.last_name = $last_name, c.birthday = $birthday, c.description = $description";
            await using var session = _driver.AsyncSession(o => o.WithDatabase(_database));
            await session.ExecuteWriteAsync(async wa =>
            {
                await wa.RunAsync(query, new
                {
                    id = creator.Id,
                    first_name = creator.First_name,
                    last_name = creator.Last_name,
                    birthday = creator.Birthday,
                    description = creator.Description
                });
            });
        }
    }
}
