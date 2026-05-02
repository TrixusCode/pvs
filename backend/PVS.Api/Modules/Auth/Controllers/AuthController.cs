using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PVS.Api.Common;
using PVS.Api.Models;
using PVS.Api.Modules.Auth.Dtos;
using PVS.Api.Modules.Auth.Services;

namespace PVS.Api.Modules.Auth.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly IAuthService _authService;
    private readonly JwtSettings _jwtSettings;

    public AuthController(IConfiguration configuration, IAuthService authService, IOptions<JwtSettings> jwtSettings)
    {
        _configuration = configuration;
        _authService = authService;
        _jwtSettings = jwtSettings.Value;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(new ApiResponse { Success = false, Message = "Invalid input" });

        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            return BadRequest(new ApiResponse { Success = false, Message = "Email and password are required" });

        var user = await _authService.AuthenticateAsync(request.Email, request.Password);
        if (user == null)
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
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(new ApiResponse { Success = false, Message = "Invalid input" });

        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            return BadRequest(new ApiResponse { Success = false, Message = "Email and password are required" });

        if (string.IsNullOrWhiteSpace(request.FirstName) || string.IsNullOrWhiteSpace(request.LastName))
            return BadRequest(new ApiResponse { Success = false, Message = "First name and last name are required" });

        if (request.Password.Length < 6)
            return BadRequest(new ApiResponse { Success = false, Message = "Password must be at least 6 characters" });

        try
        {
            var user = await _authService.RegisterAsync(request);
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
        catch (InvalidOperationException ex)
        {
            return Conflict(new ApiResponse { Success = false, Message = ex.Message });
        }
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> GetCurrentUser()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userId, out var id))
            return Unauthorized(new ApiResponse { Success = false, Message = "Invalid user" });

        var user = await _authService.GetUserByIdAsync(id);
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
    public async Task<IActionResult> RefreshToken()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userId, out var id))
            return Unauthorized(new ApiResponse { Success = false, Message = "Invalid user" });

        var user = await _authService.GetUserByIdAsync(id);
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
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(new ApiResponse { Success = false, Message = "Invalid input" });

        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userId, out var id))
            return Unauthorized(new ApiResponse { Success = false, Message = "Invalid user" });

        if (request.NewPassword.Length < 6)
            return BadRequest(new ApiResponse { Success = false, Message = "New password must be at least 6 characters" });

        var success = await _authService.ChangePasswordAsync(id, request.CurrentPassword, request.NewPassword);
        if (!success)
            return BadRequest(new ApiResponse { Success = false, Message = "Current password is incorrect or user not found" });

        return Ok(new ApiResponse
        {
            Success = true,
            Message = "Password changed successfully"
        });
    }

    private string GenerateJwtToken(User user)
    {
        if (user == null)
            throw new Exception("USER IS NULL");

        if (_jwtSettings == null)
            throw new Exception("JWT SETTINGS NOT BOUND");

        if (string.IsNullOrWhiteSpace(_jwtSettings.Secret))
            throw new Exception("JWT SECRET IS EMPTY");

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_jwtSettings.Secret));

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email ?? ""),
            new Claim(ClaimTypes.Name, $"{user.FirstName ?? ""} {user.LastName ?? ""}".Trim()),
            new Claim(ClaimTypes.Role, user.Role ?? "User")
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(24),
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience,
            SigningCredentials = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256Signature)
        };
        Console.WriteLine("SIGNING SECRET: " + _jwtSettings.Secret);
      
        return new JwtSecurityTokenHandler()
            .WriteToken(new JwtSecurityTokenHandler().CreateToken(tokenDescriptor));
    }
}
