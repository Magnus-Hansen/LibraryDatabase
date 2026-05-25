using LibraryAPI.DTOs;
using LibraryAPI.Services.Graph.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LibraryAPI.Controllers
{
    [Route("api/neo4j/Items")]
    [ApiController]
    public class ItemsControllerGraph : ControllerBase
    {
        private readonly IItemServiceGraph _itemService;

        public ItemsControllerGraph(IItemServiceGraph itemService)
        {
            _itemService = itemService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1)
        {
            var result = await _itemService.GetAllAsync(page);
            return Ok(result);
        }
        [HttpGet("type/{mediaType}")]
        public async Task<IActionResult> GetByMediatype(string mediaType, [FromQuery] int page = 1)
        {
            var result = await _itemService.GetByMediatypeAsync(mediaType, page);

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _itemService.GetByIdAsync(id);
            if (item == null)
                return NotFound();

            return Ok(item);
        }

        [HttpPost]
        public async Task<ActionResult<ItemDetailsDto>> AddAsync(CreateItemDto dto)
        {
            var createdItem = await _itemService.AddAsync(dto);

            return Ok(createdItem);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(int id, UpdateItemDto dto)
        {
            var updated = await _itemService.UpdateAsync(id, dto);

            if (!updated)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var deleted = await _itemService.DeleteAsync(id);

            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
