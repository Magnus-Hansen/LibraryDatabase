using LibraryAPI.DTOs;
using LibraryAPI.Services;
using Microsoft.AspNetCore.Mvc;


namespace LibraryAPI.Controllers;

[Route("api/mongo/Items")]
[ApiController]
public class MongoItemsController : ControllerBase
{
    private readonly MongoItemService itemService;

    public enum MediaType
    {
        Book,
        Boardgame
    }
    

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

    [HttpGet("{mediaType}/{pageNumber}")]
    public async Task<ActionResult<List<ItemDto>>> GetByMediaType(
        MediaType mediaType,
        int pageNumber = 1)
    {
        var items = await itemService.GetByMediaType(mediaType.ToString(), pageNumber);

        return Ok(items);
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
