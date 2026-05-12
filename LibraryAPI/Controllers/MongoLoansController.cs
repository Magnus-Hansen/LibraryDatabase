using LibrarySQLBackend.Models;
using Microsoft.AspNetCore.Mvc;

namespace LibraryAPI.Controllers;

[Route("api/mongo/loans")]
[ApiController]
public class MongoLoansController : ControllerBase
{
    private readonly MongoRepository<LoansMongo> _repository;

    public MongoLoansController(MongoDbContext context)
    {
        _repository = new MongoRepository<LoansMongo>(context, "Loans");
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _repository.GetAllAsync());
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var loan = await _repository.GetByIdAsync(id);

        if (loan == null)
            return NotFound();

        return Ok(loan);
    }

    [HttpPost]
    public async Task<IActionResult> Create(LoansMongo loan)
    {
        var created = await _repository.CreateAsync(loan);
        return Ok(created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, LoansMongo loan)
    {
        var existing = await _repository.GetByIdAsync(id);
        loan._id = existing._id; // Preserve the MongoDB ObjectId for the update
        var updated = await _repository.UpdateAsync(id, loan);

        if (!updated)
            return NotFound();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _repository.DeleteAsync(id);

        if (!deleted)
            return NotFound();

        return Ok();
    }
}