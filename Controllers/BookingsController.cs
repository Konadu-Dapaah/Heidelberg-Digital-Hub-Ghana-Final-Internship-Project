using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Commute360.Data;
using Commute360.Models;

namespace Commute360.Controllers
{
    public class CreateBookingDto
    {
        public int RouteId { get; set; }
        public int BoardingStopId { get; set; }
        public int DropOffStopId { get; set; }
        public List<string> Days { get; set; } = new List<string>();
    }

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

        [HttpPost]
        public async Task<IActionResult> CreateBooking([FromBody] CreateBookingDto dto)
        {
            if (dto == null)
            {
                return BadRequest(new { Message = "Invalid or missing payload." });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Extract user ID from token safely
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(new { Message = "Invalid or expired token." });
            }

            // Verify route existence and include stops
            var route = await _context.Routes
                .Include(r => r.Stops)
                .FirstOrDefaultAsync(r => r.Id == dto.RouteId);

            if (route == null)
            {
                return NotFound(new { Message = "Selected route was not found." });
            }

            // Validate stops exist on this specific route
            var boardingStop = route.Stops.FirstOrDefault(s => s.Id == dto.BoardingStopId);
            var dropOffStop = route.Stops.FirstOrDefault(s => s.Id == dto.DropOffStopId);

            if (boardingStop == null || dropOffStop == null)
            {
                return BadRequest(new { Message = "Invalid boarding or drop-off stop selected for this route." });
            }

            if (dto.BoardingStopId == dto.DropOffStopId)
            {
                return BadRequest(new { Message = "Boarding and drop-off stops cannot be identical." });
            }

            // Create and save booking
            var booking = new Booking
            {
                UserId = userId,
                RouteId = dto.RouteId,
                BoardingStopId = dto.BoardingStopId,
                DropOffStopId = dto.DropOffStopId,
                Days = dto.Days != null && dto.Days.Any() ? string.Join(",", dto.Days) : string.Empty,
                CreatedAt = DateTime.UtcNow
            };

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Booking confirmed successfully!", BookingId = booking.Id });
        }
    }
}