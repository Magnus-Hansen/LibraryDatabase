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
                var loanDate = node.Properties["loan_date"].As<LocalDateTime>();
                var dueDate = node.Properties["due_date"].As<LocalDateTime>();
                return new Loan
                {
                    Id = node.Properties["id"].As<int>(),
                    LoanDate = loanDate.ToDateTime(),
                    DueDate = dueDate.ToDateTime(),
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
        public async Task<bool> PayFineAsync(int fineId)
        {
            await using var session = _driver.AsyncSession(o => o.WithDatabase(_database));

            var query = @"
                MATCH (f:Fine {id: $fineId})
                WHERE f.status IN ['unpaid', 'late']
                SET f.status = 'paid'
                RETURN f
            ";

            var result = await session.RunAsync(query, new { fineId });
            return await result.FetchAsync();
        }
        public async Task<bool> MarkLoanOverdueAndCreateFineAsync(int loanId)
        {
            await using var session = _driver.AsyncSession(o => o.WithDatabase(_database));

            return await session.ExecuteWriteAsync(async tx =>
            {
                var result = await tx.RunAsync(@"
                    MATCH (l:Loan {id: $loanId})
                    SET l.status = 'overdue'
                    WITH l
                    OPTIONAL MATCH (l)-[:HAS_FINE]->(f:Fine)
                    WITH l, count(f) AS fineCount
                    WHERE fineCount = 0
                    CREATE (fine:Fine {
                        amount: 100.0,
                        status: 'unpaid',
                        created_date: datetime(),
                        due_date: datetime() + duration({days: 14}),
                        paid_date: null
                    })
                    CREATE (l)-[:HAS_FINE]->(fine)
                    RETURN l
                ", new { loanId });

                return await result.FetchAsync();
            });
        }
    }
}
