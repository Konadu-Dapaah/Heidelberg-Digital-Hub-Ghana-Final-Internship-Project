namespace Commute360.DTOs;
// accepts user registration data from the client and sends it to the server for processing
public class RegisterDto
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}