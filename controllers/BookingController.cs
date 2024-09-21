using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ConferenceRoomBookingAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

[Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public BookingController(ApplicationDbContext context)
        {
            _context = context;
        }

        // POST: api/booking/book
        [HttpPost("book")]
        public async Task<IActionResult> BookHall([FromBody] BookingRequest request)
        {
            // Перевiрка, чи iснує зал
            var hall = await _context.Halls.FindAsync(request.HallId);
            if (hall == null)
            {
                return NotFound("Зал не знайдений");
            }

            // Отримання обраних послуг
            var services = await _context.Services
                .Where(s => request.ServiceIds.Contains(s.Id))
                .ToListAsync();

            // Розрахунок базової вартостi оренди залу в залежностi від часу .Add(TimeSpan.FromHours(request.Duration));
            decimal baseRate = hall.BasePricePerHour;
            TimeSpan bookingEndTime = request.StartTime;

            if (request.StartTime >= TimeSpan.FromHours(18) && bookingEndTime <= TimeSpan.FromHours(23))
            {
                baseRate *= 0.8m; // Скидка 20% за вечірні години
            }
            else if (request.StartTime >= TimeSpan.FromHours(6) && bookingEndTime <= TimeSpan.FromHours(9))
            {
                baseRate *= 0.9m; // Скидка 10% за ранні години
            }
            else if (request.StartTime >= TimeSpan.FromHours(12) && bookingEndTime <= TimeSpan.FromHours(14))
            {
                baseRate *= 1.15m; // Накидка 15% за пiковий час
            }

            // Розрахунок вартостi послуг
            decimal serviceCost = services.Sum(s => s.Price);

            // Загальна вартiсть оренди
            decimal totalCost = baseRate * request.Duration + serviceCost;

            var booking = new Booking
            {
                HallId = request.HallId,
                Date = request.Date.ToUniversalTime(),
                StartTime = request.StartTime, // StartTime вже є TimeSpan
                Duration = request.Duration,
                TotalPrice = totalCost
            };

            // Зберігаємо запис бронювання, щоб отримати його Id
            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync(); // Після цього кроку booking.Id буде доступним

            // Тепер можна створювати пов'язані послуги з правильним BookingId
            var bookingServices = services.Select(s => new BookingService
            {
                ServiceId = s.Id,
                BookingId = booking.Id
            }).ToList();

            // Додаємо послуги до бронювання
            booking.BookingServices = bookingServices;

            // Зберігаємо оновлені дані
            _context.Bookings.Update(booking);
            await _context.SaveChangesAsync();

            return Ok(new { BookingId = booking.Id, TotalPrice = totalCost });

        }

        [HttpPost("search-available-halls")]
        public async Task<IActionResult> SearchAvailableHalls([FromBody] SearchAvailableHallsRequest request)
        {
            // Перевiрка дати та часу на валидність
            var bookingStart = DateTime.SpecifyKind(request.Date.Date + request.StartTime, DateTimeKind.Utc);
            var bookingEnd = DateTime.SpecifyKind(request.Date.Date + request.EndTime, DateTimeKind.Utc);

            // Перевiрка на валидність часу
            if (bookingEnd <= bookingStart)
            {
                return BadRequest("EndTime повинен бути пізніше StartTime.");
            }

            // Отримання усіх залiв з необхідною вмiстимiстю
            var halls = await _context.Halls
                .Where(h => h.Capacity >= request.RequiredCapacity)
                .ToListAsync();

            // Отримання усіх бронювання, які перетинаються з вказаним часом
            var bookings = await _context.Bookings
                .Where(b => b.Date == request.Date.ToUniversalTime() 
                    && !(b.StartTime >= request.EndTime || b.EndTime <= request.StartTime)) // Перевiрка перетину часу
                .ToListAsync();

            // Фiльтрування залiв, які зайняті в вказаний промiжок часу
            var availableHalls = halls.Where(h => !bookings.Any(b => b.HallId == h.Id)).ToList();

            return Ok(availableHalls);
        }
    }
