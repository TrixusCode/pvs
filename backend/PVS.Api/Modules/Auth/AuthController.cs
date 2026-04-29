using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PVS.Api.Common;
using PVS.Api.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PVS.Api.Modules.Auth;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _configuration;

    // Mock user database
    private static readonly List<User> Users = new()
    {
        new User
        {
            Id = 1,
            Email = "admin@example.com",
            FirstName = "Admin",
            LastName = "User",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
            Role = "Admin",
            CreatedAt = DateTime.UtcNow
        },
        new User
        {
            Id = 2,
            Email = "agent@example.com",
            FirstName = "John",
            LastName = "Agent",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("agent123"),
            Role = "Agent",
            CreatedAt = DateTime.UtcNow
        }
    };

    public AuthController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(new ApiResponse { Success = false, Message = "Invalid input" });

        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            return BadRequest(new ApiResponse { Success = false, Message = "Email and password are required" });

        // Find user by email (case-insensitive)
        var user = Users.FirstOrDefault(u => u.Email.Equals(request.Email, StringComparison.OrdinalIgnoreCase));
        if (user == null)
            return Unauthorized(new ApiResponse { Success = false, Message = "Invalid email or password" });

        // Verify password
        bool passwordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
        if (!passwordValid)
            return Unauthorized(new ApiResponse { Success = false, Message = "Invalid email or password" });

        var token = GenerateJwtToken(user);

        return Ok(new ApiResponse<string>
        {
            Success = true,
            Message = "Login successful",
            Data = token
        });
    }

    [HttpPost("register")]
    public IActionResult Register([FromBody] RegisterRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(new ApiResponse { Success = false, Message = "Invalid input" });

        // Validate input
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            return BadRequest(new ApiResponse { Success = false, Message = "Email and password are required" });

        if (string.IsNullOrWhiteSpace(request.FirstName) || string.IsNullOrWhiteSpace(request.LastName))
            return BadRequest(new ApiResponse { Success = false, Message = "First name and last name are required" });

        if (request.Password.Length < 6)
            return BadRequest(new ApiResponse { Success = false, Message = "Password must be at least 6 characters" });

        // Check if user already exists
        if (Users.Any(u => u.Email.Equals(request.Email, StringComparison.OrdinalIgnoreCase)))
            return Conflict(new ApiResponse { Success = false, Message = "Email already registered" });

        // Create new user
        var user = new User
        {
            Id = Users.Max(u => u.Id) + 1,
            Email = request.Email.ToLower().Trim(),
            FirstName = request.FirstName.Trim(),
            LastName = request.LastName.Trim(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = request.Role ?? "Agent",
            CreatedAt = DateTime.UtcNow
        };

        Users.Add(user);

        var token = GenerateJwtToken(user);

        return Ok(new ApiResponse<object>
        {
            Success = true,
            Message = "Registration successful",
            Data = new
            {
                token,
                user = new
                {
                    user.Id,
                    user.Email,
                    user.FirstName,
                    user.LastName,
                    user.Role
                }
            }
        });
    }

    [HttpGet("me")]
    [Authorize]
    public IActionResult GetCurrentUser()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userId, out var id))
            return Unauthorized(new ApiResponse { Success = false, Message = "Invalid user" });

        var user = Users.FirstOrDefault(u => u.Id == id);
        if (user == null)
            return NotFound(new ApiResponse { Success = false, Message = "User not found" });

        return Ok(new ApiResponse<object>
        {
            Success = true,
            Data = new
            {
                user.Id,
                user.Email,
                user.FirstName,
                user.LastName,
                user.Role,
                user.CreatedAt
            }
        });
    }

    [HttpPost("refresh-token")]
    [Authorize]
    public IActionResult RefreshToken()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userId, out var id))
            return Unauthorized(new ApiResponse { Success = false, Message = "Invalid user" });

        var user = Users.FirstOrDefault(u => u.Id == id);
        if (user == null)
            return NotFound(new ApiResponse { Success = false, Message = "User not found" });

        var token = GenerateJwtToken(user);

        return Ok(new ApiResponse<string>
        {
            Success = true,
            Message = "Token refreshed successfully",
            Data = token
        });
    }

    [HttpPost("change-password")]
    [Authorize]
    public IActionResult ChangePassword([FromBody] ChangePasswordRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(new ApiResponse { Success = false, Message = "Invalid input" });

        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userId, out var id))
            return Unauthorized(new ApiResponse { Success = false, Message = "Invalid user" });

        var user = Users.FirstOrDefault(u => u.Id == id);
        if (user == null)
            return NotFound(new ApiResponse { Success = false, Message = "User not found" });

        // Verify current password
        if (!BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.PasswordHash))
            return Unauthorized(new ApiResponse { Success = false, Message = "Current password is incorrect" });

        if (request.NewPassword.Length < 6)
            return BadRequest(new ApiResponse { Success = false, Message = "New password must be at least 6 characters" });

        // Update password
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
        user.UpdatedAt = DateTime.UtcNow;

        return Ok(new ApiResponse
        {
            Success = true,
            Message = "Password changed successfully"
        });
    }

    private string GenerateJwtToken(User user)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var key = Encoding.ASCII.GetBytes(jwtSettings["Secret"] ?? "your-256-bit-secret-key-here!");

        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                new Claim(ClaimTypes.Role, user.Role)
            }),
            Expires = DateTime.UtcNow.AddHours(24),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}

public class LoginRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class RegisterRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Role { get; set; }
}

public class ChangePasswordRequest
{
    public string CurrentPassword { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
}
