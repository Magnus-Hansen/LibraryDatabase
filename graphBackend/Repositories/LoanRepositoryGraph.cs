using Neo4j.Driver;
using Microsoft.Extensions.Configuration;
using graphBackend.Repositories.Interfaces;
using graphBackend.Models;

namespace graphBackend.Repositories
{
    public class LoanRepositoryGraph : ILoanRepositoryGraph
    {
        private readonly IDriver _driver;
        private readonly string _database;
        public LoanRepositoryGraph(IDriver driver, IConfiguration configuration)
        {
            _driver = driver;
            _database = configuration["Neo4j:Database"] ?? "neo4j";
        }
        public async Task<Loan?> GetByIdAsync(int id)
        {
            await using var session = _driver.AsyncSession(o => o.WithDatabase(_database));
            var result = await session.RunAsync("MATCH (l:Loan {id: $id}) RETURN l", new { id });
            if (await result.FetchAsync())
            {
                var node = result.Current["l"].As<INode>();
                return new Loan
                {
                    Id = node.Properties["id"].As<int>(),
                    LoanDate = node.Properties["loan_date"].As<DateTime>(),
                    DueDate = node.Properties["due_date"].As<DateTime>(),
                    Status = node.Properties["status"].As<string>()
                };
            }
            throw new Exception("Loan not found");
        }
        public async Task<int> CreateLoanAsync(int loanerId, int inventoryId)
        {
            await using var session = _driver.AsyncSession(o => o.WithDatabase(_database));
            int nextId = await new Services.IdGenerator(_driver, _database).GetNextId("Loan");
            DateTime loanDate = DateTime.Now;
            DateTime dueDate = loanDate.AddDays(14);
            await session.RunAsync("MATCH (l:Loaner {id: $loanerId}), (i:Inventory {id: $inventoryId}) " +
                "CREATE (l)-[:LOANED]->(loan:Loan {id: $id, loan_date: $loanDate, due_date: $dueDate, status: 'active'})-[:OF]->(i)",
                new { loanerId, inventoryId, id = nextId, loanDate, dueDate });
            return nextId;
        }
        public Task ReturnLoanAsync(int loanId)
        {
            return Task.Run(async () =>
            {
                DateTime returnDate = DateTime.Now;
                await using var session = _driver.AsyncSession(o => o.WithDatabase(_database));
                await session.RunAsync("MATCH (l:Loan {id: $loanId}) " +
                    "SET l.status = 'returned', l.return_date = $returnDate", new { loanId, returnDate });
            });
        }
    }
}
