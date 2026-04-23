using graphMigrator.Models;
using Neo4j.Driver;

namespace graphMigrator
{
    public class Neo4jService
    {
        private readonly IDriver _driver;

        public Neo4jService(string uri, string user, string password)
        {
            _driver = GraphDatabase.Driver(uri, AuthTokens.Basic(user, password));
        }

        public async Task CreateUser(Loaner loaner)
        {
            var query = @" 
            MERGE (lo:loaner {id: $id})
            SET lo.first_name = $first_name, lo.last_name = $last_name, lo.cpr = $cpr, lo.tlf = $tlf, lo.email = $email, lo.password = $password";

            await using var session = _driver.AsyncSession(o => o.WithDatabase("library"));
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

            await using var session = _driver.AsyncSession(o => o.WithDatabase("library"));
            await session.ExecuteWriteAsync(async wa =>
            {
                await wa.RunAsync(query, new
                {
                    id = language.Id,
                    name = language.Name
                });
            });
        }
    }
}
