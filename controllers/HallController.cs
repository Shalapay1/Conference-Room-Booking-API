using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ConferenceRoomBookingAPI.Models;

[Route("api/[controller]")]
[ApiController]
public class HallController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public HallController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Hall>>> GetHalls()
    {
        return await _context.Halls.Include(h => h.HallServices).ToListAsync();
    }

    [HttpPost]
    public async Task<ActionResult<Hall>> PostHall(Hall hall)
    {
        _context.Halls.Add(hall);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetHalls), new { id = hall.Id }, hall);
    }

     // Додавання зала
    [HttpPost("add")]
    public async Task<IActionResult> AddHall([FromBody] CreateHallDto hallDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        //   новий зал
        var hall = new Hall
        {
            Name = hallDto.Name,
            Capacity = hallDto.Capacity,
            BasePricePerHour = hallDto.HourlyRate
        };

        //   зал в базi
        _context.Halls.Add(hall);
        await _context.SaveChangesAsync(); //   , щоб отримати ID зала

        //   вибранi послуги в зал
        foreach (var serviceId in hallDto.ServiceIds)
        {
            var service = await _context.Services.FindAsync(serviceId);
            if (service != null)
            {
                _context.HallServices.Add(new HallService { HallId = hall.Id, ServiceId = service.Id });
            }
        }

        await _context.SaveChangesAsync();

        //   підтвердження з унікальним ID
        return Ok(new { Id = hall.Id, Message = "Зал успiшно створений" });
    }
    
    // Видалення  зала за ID
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteHall(int id)
    {
        //   зал за ID
        var hall = await _context.Halls.FindAsync(id);

        //   зал не знайдений, вернем 404
        if (hall == null)
        {
            return NotFound(new { Message = "Зал не знайдений" });
        }

        //   зала
        _context.Halls.Remove(hall);
        await _context.SaveChangesAsync();

        //   підтвердження видалення
        return Ok(new { Message = "Зал успiшно видалений" });
    }

     //   Оновлення зала за ID
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateHall(int id, [FromBody] UpdateHallRequest updateRequest)
    {
        //   зал за ID
        var hall = await _context.Halls.Include(h => h.HallServices).FirstOrDefaultAsync(h => h.Id == id);

        //   зал не знайдений, вернем 404
        if (hall == null)
        {
            return NotFound(new { Message = "Зал не знайдений" });
        }

        //   поля, тільки якщо вони були передані в запитi
        if (!string.IsNullOrEmpty(updateRequest.Name))
        {
            hall.Name = updateRequest.Name;
        }

        if (updateRequest.Capacity > 0) //   , що вмiстимiсть не може бути від'ємною або 0
        {
            hall.Capacity = updateRequest.Capacity;
        }

        if (updateRequest.HourlyRate > 0) //   також не може бути від'ємною або 0
        {
            hall.BasePricePerHour = updateRequest.HourlyRate;
        }

        //   послуг, якщо передані serviceIds
        if (updateRequest.ServiceIds != null && updateRequest.ServiceIds.Count > 0)
        {
            //   старi послуг
            var existingServices = _context.HallServices.Where(hs => hs.HallId == id);
            _context.HallServices.RemoveRange(existingServices);

            //   новi послуги
            foreach (var serviceId in updateRequest.ServiceIds)
            {
                var hallService = new HallService
                {
                    HallId = hall.Id,
                    ServiceId = serviceId
                };
                _context.HallServices.Add(hallService);
            }
        }

        //   зміни
        await _context.SaveChangesAsync();

        //   підтвердження успiшного оновлення
        return Ok(new { Message = "Зал успiшно оновлений" });
    }
}
