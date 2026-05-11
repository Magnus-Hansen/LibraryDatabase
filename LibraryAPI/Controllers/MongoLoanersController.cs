using Microsoft.AspNetCore.Mvc;


namespace LibraryAPI.Controllers;

    [Route("api/mongo/loaners")]
    [ApiController]
    public class MongoLoanersController : ControllerBase
    {
        private readonly MongoRepository<LoanersMongo> _repository;

        public MongoLoanersController(MongoDbContext context)
        {
            _repository = new MongoRepository<LoanersMongo>(context, "Loaners");
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _repository.GetAllAsync());
        }

        // Needs new implementation
        /*
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var loaner = await _repository.GetByIdAsync(id);

            if (loaner == null)
                return NotFound();

            return Ok(loaner);
        }
        */

        [HttpPost]
        public async Task<IActionResult> Create(LoanersMongo loaner)
        {
            var created = await _repository.CreateAsync(loaner);
            return Ok(created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, LoanersMongo loaner)
        {
            var updated = await _repository.UpdateAsync(id, loaner);

            if (!updated)
                return NotFound();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var deleted = await _repository.DeleteAsync(id);

            if (!deleted)
                return NotFound();

            return NoContent();
        }
    
}
