using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
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
    private readonly AccountMailer _mailer;
    private readonly IConfiguration _config;

    public AuthController(AppDbContext context, TokenService tokenService, AccountMailer mailer, IConfiguration config)
    {
        _context = context;
        _tokenService = tokenService;
        _mailer = mailer;
        _config = config;
    }

    // Compares an invite code without leaking timing information.
    // A role whose code isn't configured can't be self-registered at all.
    private static bool CodeMatches(string? supplied, string? expected)
    {
        if (string.IsNullOrEmpty(supplied) || string.IsNullOrEmpty(expected)) return false;
        var a = Encoding.UTF8.GetBytes(supplied);
        var b = Encoding.UTF8.GetBytes(expected);
        return a.Length == b.Length && CryptographicOperations.FixedTimeEquals(a, b);
    }

    [HttpPost("register")]
    [EnableRateLimiting("auth")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        // 1. Duplicate email check
        if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
            return BadRequest("Email already registered.");

        if (string.IsNullOrWhiteSpace(dto.Password) || dto.Password.Length < 8)
            return BadRequest("Password must be at least 8 characters.");

        // 2. Role: anyone can register as Staff. Driver and Admin need an invite code,
        //    otherwise any visitor could create an admin account.
        var role = string.IsNullOrWhiteSpace(dto.Role) ? "Staff" : dto.Role;

        var allowedRoles = new[] { "Staff", "Driver", "Admin" };
        if (!allowedRoles.Contains(role))
            return BadRequest("Role must be Staff, Driver, or Admin.");

        if (role == "Driver" && !CodeMatches(dto.InviteCode, _config["Registration:DriverCode"]))
            return BadRequest("A valid driver invite code is required.");

        if (role == "Admin" && !CodeMatches(dto.InviteCode, _config["Registration:AdminCode"]))
            return BadRequest("A valid admin invite code is required.");

        // 3. Create the new user entity
        var user = new User
        {
            Name = dto.Name,
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Role = role,
            EmailVerified = false
        };

        // 4. Save to database
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // 5. Ask them to confirm their email (a failure here must not fail the registration)
        try { await _mailer.SendVerificationAsync(user); }
        catch (Exception ex) { Console.WriteLine($"Verification email failed: {ex.Message}"); }

        return Ok(new { message = "Registration successful." });
    }

    [HttpPost("login")]
    [EnableRateLimiting("auth")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
        if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            return Unauthorized("Invalid email or password.");

        return Ok(new { token = _tokenService.CreateToken(user), role = user.Role });
    }
}