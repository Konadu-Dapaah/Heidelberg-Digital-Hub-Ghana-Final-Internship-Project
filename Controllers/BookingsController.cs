using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Commute360.Data;
using Commute360.DTOs;
using Commute360.Models;

namespace Commute360.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BookingsController : ControllerBase
{
    private readonly AppDbContext _context;

    public BookingsController(AppDbContext context)
    {
        _context = context;
    }

    private int CurrentUserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpPost]
    public async Task<IActionResult> CreateBooking(CreateBookingDto dto)
    {
        // 1. Prevent selecting the same stop for boarding and drop-off
        if (dto.BoardingStopId == dto.DropOffStopId)
            return BadRequest("Boarding and drop-off stops must be different.");

        // 2. Fetch the route and its associated stops
        var route = await _context.Routes
            .Include(r => r.Stops)
            .FirstOrDefaultAsync(r => r.Id == dto.RouteId);

        if (route == null) 
            return NotFound("Route not found.");

        // 3. Validate that both stops belong to this route
        bool boardingValid = route.Stops.Any(s => s.Id == dto.BoardingStopId);
        bool dropOffValid = route.Stops.Any(s => s.Id == dto.DropOffStopId);

        if (!boardingValid || !dropOffValid)
            return BadRequest("Boarding or drop-off stop does not belong to this route.");

        // 4. Create and persist the booking entity
        var booking = new Booking
        {
            UserId = CurrentUserId,
            RouteId = dto.RouteId,
            BoardingStopId = dto.BoardingStopId,
            DropOffStopId = dto.DropOffStopId,
            Days = dto.Days
        };

        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync();

        return Ok(booking);
    }

    [HttpGet("mine")]
    public async Task<IActionResult> GetMyBookings()
    {
        var bookings = await _context.Bookings
            .Include(b => b.Route)
            .Include(b => b.BoardingStop)
            .Include(b => b.DropOffStop)
            .Where(b => b.UserId == CurrentUserId)
            .ToListAsync();

        return Ok(bookings);
    }
}