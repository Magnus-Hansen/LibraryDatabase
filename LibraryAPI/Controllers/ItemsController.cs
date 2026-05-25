using LibraryAPI.DTOs;
using LibraryAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LibraryAPI.Controllers
{
    [Route("api/mysql/[controller]")]
    [ApiController]
    [Authorize]
    public class ItemsController : ControllerBase
    {
        private readonly IItemService _itemService;

        public ItemsController(IItemService itemService)
        {
            _itemService = itemService;
        }


        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1)
        {
            var items = await _itemService.GetAllAsync(pageNumber);
            return Ok(items);
        }

        [HttpGet("type/{mediaType}")]
        public async Task<IActionResult> GetByMediaType(string mediaType, [FromQuery] int pageNumber = 1)
        {
            var normalizedMediaType = mediaType.ToLower();

            if (normalizedMediaType != "book" && normalizedMediaType != "boardgame")
            {
                return BadRequest(new
                {
                    message = "Invalid media type. Use 'book' or 'boardgame'."
                });
            }

            var items = await _itemService.GetByMediaTypeAsync(normalizedMediaType, pageNumber);
            return Ok(items);
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
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _itemService.DeleteAsync(id);

            if (!result.Success)
            {
                return Conflict(new { message = result.Message });
            }

            return Ok(new { message = result.Message });
        }

        [HttpPost("{id}/generate-review-summary")]
        public async Task<IActionResult> GenerateReviewSummaryAsync(int id)
        {
            try
            {
                var item = await _itemService.GenerateReviewSummaryAsync(id);

                if (item == null)
                {
                    return NotFound();
                }

                return Ok(item);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (HttpRequestException)
            {
                return StatusCode(503, new
                {
                    message = "The AI service is currently unavailable. Make sure Ollama is running."
                });
            }
        }


    }
}
