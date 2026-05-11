using Microsoft.AspNetCore.Mvc;


namespace LibraryAPI.Controllers;

[Route("api/mongo/items")]
[ApiController]
public class MongoItemsController : ControllerBase
{
    private readonly MongoRepository<ItemMongo> _repository;

    public MongoItemsController(MongoDbContext context)
    {
        _repository = new MongoRepository<ItemMongo>(context, "Items");
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _repository.GetAllAsync());
    }

    // Needs new implementation

    //[HttpGet("{id}")]
    //public async Task<IActionResult> GetById(string id)
    //{
    //    var item = await _repository.GetByIdAsync(id);

    //    if (item == null)
    //        return NotFound();

    //    return Ok(item);
    //}

    [HttpPost]
    public async Task<IActionResult> Create(ItemMongo item)
    {
        var created = await _repository.CreateAsync(item);
        return Ok(created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, ItemMongo item)
    {
        var updated = await _repository.UpdateAsync(id, item);

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
