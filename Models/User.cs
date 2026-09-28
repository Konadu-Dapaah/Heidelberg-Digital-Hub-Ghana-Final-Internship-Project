namespace Commute360.Models;

public class User
{
    //user id,name,email,harsed password,role
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Role { get; set; } = "Staff"; 

    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}