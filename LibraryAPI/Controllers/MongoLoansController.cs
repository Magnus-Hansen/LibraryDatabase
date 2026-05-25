using LibraryAPI.DTOs;
using LibraryAPI.Services;
using LibrarySQLBackend.Models;
using Microsoft.AspNetCore.Mvc;

namespace LibraryAPI.Controllers;

[Route("api/mongo/Loans")]
[ApiController]
public class MongoLoansController : ControllerBase
{
    private readonly MongoLoanService loanService;

    public MongoLoansController(MongoDbContext context)
    {
       loanService= new MongoLoanService(context);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {

        var loan = await loanService.GetByIdAsync(id);
        if (loan == null)
            return NotFound();
        return Ok(loan);

    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateLoanDto loan)
    {
        var created = await loanService.CreateAsync(loan);
        return Ok(created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, LoanDto loan)
    {
        var existing = await loanService.GetByIdAsync(id);
        var updated = await loanService.UpdateAsync(loan, id);

        if (!updated)
            return NotFound();

        return Ok(updated);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await loanService.DeleteAsync(id);

        if (!deleted)
            return NotFound();

        return Ok();
    }
}