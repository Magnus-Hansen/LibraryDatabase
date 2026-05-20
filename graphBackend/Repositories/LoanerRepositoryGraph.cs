using graphBackend.Models;
using graphBackend.Repositories.Interfaces;
using Microsoft.Extensions.Configuration;
using Neo4j.Driver;

namespace graphBackend.Repositories
{
    public class LoanerRepositoryGraph : ILoanerRepositoryGraph
    {
        private readonly IDriver _driver;
        private readonly string _database;

        public LoanerRepositoryGraph(IDriver driver, IConfiguration configuration)
        {
            _driver = driver;
            _database = configuration["Neo4j:Database"] ?? "neo4j";
        }

        public async Task<IEnumerable<Loaner>> GetAllAsync()
        {
            var loaners = new List<Loaner>();
            await using var session = _driver.AsyncSession(o => o.WithDatabase(_database));
            var result = await session.RunAsync("MATCH (l:Loaner) RETURN l");

            while (await result.FetchAsync())
            {
                var node = result.Current["l"].As<INode>();

                loaners.Add(new Loaner
                {
                    Id = node.Properties["id"].As<int>(),
                    FirstName = node.Properties["first_name"].As<string>(),
                    LastName = node.Properties["last_name"].As<string>(),
                    Email = node.Properties["email"].As<string>(),
                    Cpr = node.Properties["cpr"].As<string>(),
                    Password = node.Properties["password"].As<string>(),
                    Tlf = node.Properties["tlf"].As<string>()
                });
            }
            return loaners;
        }

        public async Task<Loaner?> GetByIdAsync(int id)
        {
            await using var session = _driver.AsyncSession(o => o.WithDatabase(_database));
            var result = await session.RunAsync("MATCH (l:Loaner {id: $id}) RETURN l", new { id });
            if (await result.FetchAsync())
            {
                var node = result.Current["l"].As<INode>();

                return new Loaner
                {
                    Id = node.Properties["id"].As<int>(),
                    FirstName = node.Properties["first_name"].As<string>(),
                    LastName = node.Properties["last_name"].As<string>(),
                    Email = node.Properties["email"].As<string>(),
                    Cpr = node.Properties["cpr"].As<string>(),
                    Password = node.Properties["password"].As<string>(),
                    Tlf = node.Properties["tlf"].As<string>()
                };
            }
            return null;
        }

        public Task<Loaner?> GetByEmailAsync(string email)
        {
            return Task.Run(async () =>
            {
                await using var session = _driver.AsyncSession(o => o.WithDatabase(_database));
                var result = await session.RunAsync("MATCH (l:Loaner {email: $email}) RETURN l", new { email });
                if (await result.FetchAsync())
                {
                    var node = result.Current["l"].As<INode>();
                    return new Loaner
                    {
                        Id = node.Properties["id"].As<int>(),
                        FirstName = node.Properties["first_name"].As<string>(),
                        LastName = node.Properties["last_name"].As<string>(),
                        Email = node.Properties["email"].As<string>(),
                        Cpr = node.Properties["cpr"].As<string>(),
                        Password = node.Properties["password"].As<string>(),
                        Tlf = node.Properties["tlf"].As<string>()
                    };
                }
                return null;
            });
        }

        public async Task<Loaner> AddAsync(Loaner loaner)
        {
            await using var session = _driver.AsyncSession(o => o.WithDatabase(_database));
            int nextId = await new Services.IdGenerator(_driver, _database).GetNextId("Loaner");
            var result = await session.ExecuteWriteAsync(async transaction =>
            {
                var cursor  = await session.RunAsync(@"
                CREATE (l:Loaner {
                    id: $id,
                    first_name: $first_name,
                    last_name: $last_name,
                    email: $email,
                    cpr: $cpr,
                    password: $password,
                    tlf: $tlf
                })",
                new
                {
                    id = nextId,
                    first_name = loaner.FirstName,
                    last_name = loaner.LastName,
                    email = loaner.Email,
                    cpr = loaner.Cpr,
                    password = loaner.Password,
                    tlf = loaner.Tlf
                });
                return await cursor.SingleAsync(record => record["l"].As<INode>());
            });
            return new Loaner
            {
                Id = result.Properties["id"].As<int>(),
                FirstName = result.Properties["first_name"].As<string>(),
                LastName = result.Properties["last_name"].As<string>(),
                Email = result.Properties["email"].As<string>(),
                Cpr = result.Properties["cpr"].As<string>(),
                Password = result.Properties["password"].As<string>(),
                Tlf = result.Properties["tlf"].As<string>()
            };
        }

        public async Task<bool> UpdateAsync(Loaner loaner)
        {
            await using var session = _driver.AsyncSession(o => o.WithDatabase(_database));
            await session.ExecuteWriteAsync(async transaction =>
            {
                await session.RunAsync(@"
                MATCH (l:Loaner {id: $id})
                SET
                    l.first_name = $first_name,
                    l.last_name = $last_name,
                    l.email = $email,
                    l.cpr = $cpr,
                    l.password = $password,
                    l.tlf = $tlf",
                new
                {
                    id = loaner.Id,
                    first_name = loaner.FirstName,
                    last_name = loaner.LastName,
                    email = loaner.Email,
                    cpr = loaner.Cpr,
                    password = loaner.Password,
                    tlf = loaner.Tlf
                });
            });
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            await using var session = _driver.AsyncSession(o => o.WithDatabase(_database));
            await session.ExecuteWriteAsync(async transaction =>
            {
                await session.RunAsync("MATCH (l:Loaner {id: $id}) DETACH DELETE l", new { id });
            });
            return true;
        }
    }
}
