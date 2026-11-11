using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using netCurdApi.Auth;
using netCurdApi.Data;
using netCurdApi.Dtos;
using netCurdApi.Models;

namespace netCurdApi.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly JwtService _jwtService;

    public AuthController(AppDbContext context, JwtService jwtService)
    {
        _context = context;
        _jwtService = jwtService;
    }

    // POST: api/auth/register
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        var username = request.Username.Trim();
        var email = request.Email.Trim();
        var usernameLower = username.ToLower();
        var emailLower = email.ToLower();

        if (await _context.Users.AnyAsync(u => u.Username.ToLower() == usernameLower))
            return Conflict(new { message = "Username is already taken." });

        if (await _context.Users.AnyAsync(u => u.Email.ToLower() == emailLower))
            return Conflict(new { message = "Email is already registered." });

        var user = new User
        {
            Username = username,
            Email = email,
            PasswordHash = PasswordHasher.Hash(request.Password),
            Status = "A",
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return StatusCode(StatusCodes.Status201Created, BuildAuthResponse(user));
    }

    // POST: api/auth/login
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var identifier = request.UsernameOrEmail.Trim().ToLower();

        var user = await _context.Users.FirstOrDefaultAsync(u =>
            u.Username.ToLower() == identifier || u.Email.ToLower() == identifier);

        if (user is null || !PasswordHasher.Verify(request.Password, user.PasswordHash))
            return Unauthorized(new { message = "Invalid username/email or password." });

        if (!string.Equals(user.Status, "A", StringComparison.OrdinalIgnoreCase))
            return StatusCode(StatusCodes.Status403Forbidden, new { message = "This account is inactive." });

        return Ok(BuildAuthResponse(user));
    }

    private AuthResponse BuildAuthResponse(User user)
    {
        var (token, expiresAt) = _jwtService.GenerateToken(user);

        return new AuthResponse
        {
            Token = token,
            ExpiresAt = expiresAt,
            User = new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Status = user.Status,
                CreatedAt = user.CreatedAt
            }
        };
    }
}
