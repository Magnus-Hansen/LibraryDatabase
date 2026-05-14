using graphBackend.Models;
using graphBackend.Repositories.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualBasic;
using Neo4j.Driver;

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
                var LoanDate = node.Properties["loan_date"].As<ZonedDateTime>();
                var DueDate = node.Properties["due_date"].As<ZonedDateTime>();
                return new Loan
                {
                    Id = node.Properties["id"].As<int>(),
                    LoanDate = LoanDate.ToDateTimeOffset().UtcDateTime,
                    DueDate = DueDate.ToDateTimeOffset().UtcDateTime,
                    Status = node.Properties["status"].As<string>()
                };
            }
            throw new Exception("Loan not found");
        }
        public async Task<int> CreateLoanAsync(int loanerId, int inventoryId)
        {
            await using var session = _driver.AsyncSession(o => o.WithDatabase(_database));
            int nextId = await new Services.IdGenerator(_driver, _database).GetNextId("Loan");

            DateTime loanDate = DateTime.UtcNow;
            DateTime dueDate = loanDate.AddDays(14);

            var query = @"
                MATCH (l:Loaner {id: $loanerId})
                MATCH (i:Inventory {id: $inventoryId})
                CREATE (l)-[:MADE_LOAN]->(loan:Loan {
                    id: $id,
                    loan_date: datetime($loanDate),
                    due_date: datetime($dueDate),
                    status: 'active'
                })-[:LOANS_FROM]->(i)
                RETURN loan.id AS loanId
            ";
            var result = await session.RunAsync(query, new
            {
                loanerId,
                inventoryId,
                id = nextId,
                loanDate,
                dueDate
            });
            return nextId;
        }
        public Task ReturnLoanAsync(int loanId)
        {
            return Task.Run(async () =>
            {
                DateTime returnDate = DateTime.UtcNow;
                await using var session = _driver.AsyncSession(o => o.WithDatabase(_database));
                await session.RunAsync("MATCH (l:Loan {id: $loanId}) " +
                    "SET l.status = 'returned', l.return_date = $returnDate", new { loanId, returnDate });
            });
        }
    }
}
