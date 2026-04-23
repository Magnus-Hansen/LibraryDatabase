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
            SET i.name = $name, i.release_year = $release_year, i.description = $description, i.review_summary = $review_summary, i.media_type = $media_type, i.image = $image, i.language_id = $language_id , i.publisher_id = $publisher_id, i.average_stars = $average_stars";
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
    }
}
