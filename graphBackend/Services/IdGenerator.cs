using Neo4j.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace graphBackend.Services
{
    public class IdGenerator
    {
        private readonly IDriver _driver;
        private readonly string _database;

        public IdGenerator(IDriver driver, string database)
        {
            _driver = driver;
            _database = database;
        }

        public async Task<int> GetNextId(string counterName)
        {
            await using var session = _driver.AsyncSession(o => o.WithDatabase(_database));

            return await session.ExecuteWriteAsync(async tx =>
            {
                var result = await tx.RunAsync(@"
                MATCH (c:Counter {name: $name})
                SET c.value = c.value + 1
                RETURN c.value AS nextId",
                new { name = counterName });

                var record = await result.SingleAsync();
                return record["nextId"].As<int>();
            });
        }
    }
}
