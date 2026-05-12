using LibrarySQLBackend.Models;
using Microsoft.AspNetCore.Mvc;

namespace LibraryAPI.Controllers;

[Route("api/mongo/reservations")]
[ApiController]
public class MongoReservationsController : ControllerBase
{
    private readonly MongoRepository<ReservationsMongo> _repository;

    public MongoReservationsController(MongoDbContext context)
    {
        _repository = new MongoRepository<ReservationsMongo>(context, "Reservations");
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _repository.GetAllAsync());
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var reservation = await _repository.GetByIdAsync(id);

        if (reservation == null)
            return NotFound();

        return Ok(reservation);
    }

    [HttpPost]
    public async Task<IActionResult> Create(ReservationsMongo reservation)
    {
        var created = await _repository.CreateAsync(reservation);
        return Ok(created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, ReservationsMongo reservation)
    {
        var existing = await _repository.GetByIdAsync(id);
        reservation._id = existing._id; // Preserve the MongoDB ObjectId for the update
        var updated = await _repository.UpdateAsync(id, reservation);

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