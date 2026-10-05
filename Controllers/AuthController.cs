using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Commute360.Data;
using Commute360.DTOs;
using Commute360.Models;
using Commute360.Services;

namespace Commute360.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly TokenService _tokenService;

    public AuthController(AppDbContext context, TokenService tokenService)
    {
        _context = context;
        _tokenService = tokenService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        // 1. Duplicate email check
        if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
            return BadRequest("Email already registered.");

        // 2. Role validation check (Updated: Added 'Admin')
        var allowedRoles = new[] { "Staff", "Driver", "Admin" };
        if (!allowedRoles.Contains(dto.Role))
            return BadRequest("Role must be Staff, Driver, or Admin.");

        // 3. Create the new user entity
        var user = new User
        {
            Name = dto.Name,
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Role = dto.Role
        };

        // 4. Save to database
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Registration successful." });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
        if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            return Unauthorized("Invalid email or password.");

        return Ok(new { token = _tokenService.CreateToken(user), role = user.Role });
    }
}