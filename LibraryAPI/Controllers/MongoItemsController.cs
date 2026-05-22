using LibraryAPI.DTOs;
using LibraryAPI.Services;
using Microsoft.AspNetCore.Mvc;


namespace LibraryAPI.Controllers;

[Route("api/mongo/items")]
[ApiController]
public class MongoItemsController : ControllerBase
{
    private readonly MongoItemService itemService;

    public MongoItemsController(MongoDbContext context)
    {
        itemService = new MongoItemService(context);
    }

    [HttpGet("page/{page:int}")]
    public async Task<IActionResult> GetPage(int page)
    {
        return Ok(await itemService.GetPageAsync(page));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var item = await itemService.GetByIdAsync(id);

        if (item == null)
            return NotFound();

        return Ok(item);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateItemDto item)
    {
        var created = await itemService.CreateAsync(item);
        return Ok(created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateItemDto item)
    {
        var existingItem = await itemService.GetByIdAsync(id);
        var updated = await itemService.UpdateAsync(item, id);

        if (!updated)
            return NotFound();

        return Ok(item);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await itemService.DeleteAsync(id);

        if (!deleted)
            return NotFound();

        return Ok();
    }
}
