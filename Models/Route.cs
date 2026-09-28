namespace Commute360.Models;

public class Route
{
    //route id,
    public int Id { get; set; }
    public string Origin { get; set; } = string.Empty;
    public string Destination { get; set; } = string.Empty;
    public DateTime ScheduleTime { get; set; }
    public int Capacity { get; set; }

    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}