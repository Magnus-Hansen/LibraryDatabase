using Microsoft.AspNetCore.Mvc;

namespace LibraryAPI.Controllers;

[Route("api/mongo/inventory")]
[ApiController]
public class MongoInventoryController : ControllerBase
{
    private readonly MongoRepository<InventoryMongo> _repository;

    public MongoInventoryController(MongoDbContext context)
    {
        _repository = new MongoRepository<InventoryMongo>(context, "Inventory");
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _repository.GetAllAsync());
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var inventory = await _repository.GetByIdAsync(id);

        if (inventory == null)
            return NotFound();

        return Ok(inventory);
    }

    [HttpPost]
    public async Task<IActionResult> Create(InventoryMongo inventory)
    {
        var created = await _repository.CreateAsync(inventory);
        return Ok(created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, InventoryMongo inventory)
    {
        var existing = await _repository.GetByIdAsync(id);
        inventory._id = existing._id; // Preserve the MongoDB ObjectId for the update
        var updated = await _repository.UpdateAsync(id, inventory);

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