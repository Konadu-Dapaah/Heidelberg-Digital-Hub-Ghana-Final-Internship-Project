using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Commute360.Data;
using Commute360.DTOs;
using Commute360.Models;

namespace Commute360.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RoutesController : ControllerBase
{
    private readonly AppDbContext _context;

    public RoutesController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetRoutes()
    {
        var routes = await _context.Routes
            .Include(r => r.Stops.OrderBy(s => s.Order))
            .ToListAsync();

        return Ok(routes);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetRoute(int id)
    {
        var route = await _context.Routes
            .Include(r => r.Stops.OrderBy(s => s.Order))
            .FirstOrDefaultAsync(r => r.Id == id);

        if (route == null) return NotFound();
        return Ok(route);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> CreateRoute(CreateRouteDto dto)
    {
        if (dto.Capacity <= 0)
            return BadRequest("Capacity must be greater than zero.");

        if (string.IsNullOrWhiteSpace(dto.Origin) || string.IsNullOrWhiteSpace(dto.Destination))
            return BadRequest("Origin and destination are required.");

        var route = new Commute360.Models.Route
        {
            Origin = dto.Origin,
            Destination = dto.Destination,
            ScheduleTime = dto.ScheduleTime,
            Capacity = dto.Capacity
        };

        _context.Routes.Add(route);
        await _context.SaveChangesAsync();

        return Ok(route);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("{id}/stops")]
    public async Task<IActionResult> AddStop(int id, CreateStopDto dto)
    {
        var route = await _context.Routes.FindAsync(id);
        if (route == null) return NotFound("Route not found.");

        var stop = new Stop
        {
            RouteId = id,
            Name = dto.Name,
            Latitude = dto.Latitude,
            Longitude = dto.Longitude,
            Order = dto.Order
        };

        _context.Stops.Add(stop);
        await _context.SaveChangesAsync();

        return Ok(stop);
    }
}