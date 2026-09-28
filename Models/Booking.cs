namespace Commute360.Models;

public class Booking
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User? User { get; set; }

    public int RouteId { get; set; }
    public Route? Route { get; set; }
    //status of booking; confirmed,cancelled
    public string Status { get; set; } = "Confirmed"; 
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}