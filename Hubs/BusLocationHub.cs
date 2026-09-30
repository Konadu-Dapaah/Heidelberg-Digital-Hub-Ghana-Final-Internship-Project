using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Commute360.Data;
using Commute360.Services;

namespace Commute360.Hubs;

public class BusLocationHub : Hub
{
    private readonly AppDbContext _context;

    public BusLocationHub(AppDbContext context)
    {
        _context = context;
    }
// 
    public async Task JoinRoute(string routeId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, routeId);
    }

    public async Task SendLocation(string routeId, double lat, double lng)
    {
        await Clients.Group(routeId).SendAsync("ReceiveLocation", lat, lng);

        int routeIdInt = int.Parse(routeId);
        var stops = await _context.Stops
            .Where(s => s.RouteId == routeIdInt)
            .ToListAsync();

        foreach (var stop in stops)
        {
            double distance = DistanceService.CalculateDistanceKm(lat, lng, stop.Latitude, stop.Longitude);
            if (distance <= 1.0)
            {
                await Clients.Group(routeId).SendAsync("BusNearStop", stop.Name, distance);
            }
        }
    }
}