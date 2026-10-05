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

    private int CurrentUserId =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpPost]
    public async Task<IActionResult> CreateBooking(CreateBookingDto dto)
    {
        var route = await _context.Routes
            .Include(r => r.Stops)
            .FirstOrDefaultAsync(r => r.Id == dto.RouteId);

        if (route == null) return NotFound("Route not found.");

        bool boardingValid = route.Stops.Any(s => s.Id == dto.BoardingStopId);
        bool dropOffValid = route.Stops.Any(s => s.Id == dto.DropOffStopId);
        if (!boardingValid || !dropOffValid)
            return BadRequest("Boarding or drop-off stop does not belong to this route.");

        if (dto.BoardingStopId == dto.DropOffStopId)
            return BadRequest("Boarding and drop-off stops must be different.");

        // Seat capacity check
        int confirmedCount = await _context.Bookings
            .CountAsync(b => b.RouteId == dto.RouteId && b.Status == "Confirmed");

        if (confirmedCount >= route.Capacity)
            return BadRequest("This route is fully booked.");

        // A user has only one active booking at a time — replace any existing one
        var existingBookings = await _context.Bookings
            .Where(b => b.UserId == CurrentUserId && b.Status == "Confirmed")
            .ToListAsync();

        foreach (var existing in existingBookings)
        {
            existing.Status = "Cancelled";
        }

        var booking = new Booking
        {
            UserId = CurrentUserId,
            RouteId = dto.RouteId,
            BoardingStopId = dto.BoardingStopId,
            DropOffStopId = dto.DropOffStopId,
            Days = string.Join(",", dto.Days)
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
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync();

        return Ok(bookings);
    }
}