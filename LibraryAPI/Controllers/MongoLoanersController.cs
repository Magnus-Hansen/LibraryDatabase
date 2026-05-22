using LibraryAPI.DTOs;
using LibraryAPI.Services;
using LibrarySQLBackend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Text.Json.Serialization;


namespace LibraryAPI.Controllers;

    [Route("api/mongo/loaners")]
    [ApiController]
    public class MongoLoanersController : ControllerBase
    {
    private readonly MongoLoanerService loanerService;

        public MongoLoanersController(MongoDbContext context)
        {
            loanerService = new MongoLoanerService(context);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
        {
            var loaner = await loanerService.GetByIdAsync(id);
            if (loaner == null)
            return NotFound();

            return Ok(loaner);
        }
        [HttpPost]
        public async Task<IActionResult> Create(RegisterLoanerDto loaner)
        {
            var created = await loanerService.CreateAsync(loaner);
            return Ok(created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, LoanerDto loaner)
        {
        var existing = await loanerService.GetByIdAsync(id);
        if (existing == null)
            return NotFound();

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await loanerService.DeleteAsync(id);

            if (!deleted)
                return NotFound();

            return Ok();
        }
    
}
