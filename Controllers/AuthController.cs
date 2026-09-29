using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Commute360.Data;
using Commute360.DTOs;
using Commute360.Models;
using Commute360.Services;

namespace Commute360.Controllers;

[ApiController]
[Route("api/[controller]")]
// The AuthController class handles user authentication and registration, providing endpoints for user login and registration. It interacts with the AppDbContext to manage user data and uses the TokenService to generate JWT tokens for authenticated users.
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly TokenService _tokenService;
// The constructor initializes the AuthController with the application's database context and token service, allowing it to access user data and generate JWT tokens for authentication.
    public AuthController(AppDbContext context, TokenService tokenService)
    {
        _context = context;
        _tokenService = tokenService;
    }
// The Register method handles user registration by accepting a RegisterDto object, checking for existing users with the same email, hashing the password, and saving the new user to the database. It returns a success message upon successful registration or an error message if the email is already registered.
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        // Check if the email is already registered
        if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
            return BadRequest("Email already registered.");
        // Create a new user with the provided registration data and hash the password for security
        var user = new User
        {
            Name = dto.Name,
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
        };
        // Save the new user to the database
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Registration successful." });
    }
    // The Login method handles user authentication by accepting a LoginDto object, verifying the user's email and password, and returning a JWT token if the credentials are valid. If the credentials are invalid, it returns an unauthorized error message.
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        // Find the user by email and verify the password using BCrypt for secure password comparison
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
        // If the user is not found or the password is incorrect, return an unauthorized response
        if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            return Unauthorized("Invalid email or password.");

        return Ok(new { token = _tokenService.CreateToken(user) });
    }
}