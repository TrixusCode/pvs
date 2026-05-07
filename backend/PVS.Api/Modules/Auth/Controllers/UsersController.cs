using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PVS.Api.Common;
using PVS.Api.Data;
using PVS.Api.Models;
using PVS.Api.Modules.Auth.Dtos;
using PVS.Api.Modules.Auth.Mappers;
using BCrypt.Net;

namespace PVS.Api.Modules.Auth.Controllers;

[ApiController]
[Route("api/users")]
[Authorize]
public class UsersController(AppDbContext context) : ControllerBase
{
    [HttpGet]
    [Authorize(Roles = "Admin,Manager")]
    public IActionResult GetAll()
    {
        var users = context.Users.ToList();
        return Ok(new ApiResponse<List<UserDto>>
        {
            Success = true,
            Data = users.Select(user => user.ToDto()).ToList()
        });
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,Manager")]
    public IActionResult GetById(int id)
    {
        var user = context.Users.Find(id);
        if (user == null)
            return NotFound(new ApiResponse { Success = false, Message = "User not found" });

        return Ok(new ApiResponse<UserDto>
        {
            Success = true,
            Data = user.ToDto()
        });
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Create([FromBody] CreateUserRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(new ApiResponse { Success = false, Message = "Invalid input" });

        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            return BadRequest(new ApiResponse { Success = false, Message = "Email and password are required" });

        if (string.IsNullOrWhiteSpace(request.FirstName) || string.IsNullOrWhiteSpace(request.LastName))
            return BadRequest(new ApiResponse { Success = false, Message = "First name and last name are required" });

        if (request.Password.Length < 6)
            return BadRequest(new ApiResponse { Success = false, Message = "Password must be at least 6 characters" });

        // Check if email already exists
        if (context.Users.Any(u => u.Email == request.Email))
            return Conflict(new ApiResponse { Success = false, Message = "Email already exists" });

        var user = new User
        {
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = request.Role ?? "Agent",
            CreatedAt = DateTime.UtcNow
        };

        context.Users.Add(user);
        await context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = user.Id },
            new ApiResponse<UserDto>
            {
                Success = true,
                Message = "User created successfully",
                Data = user.ToDto()
            });
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateUserRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(new ApiResponse { Success = false, Message = "Invalid input" });

        var user = await context.Users.FindAsync(id);
        if (user == null)
            return NotFound(new ApiResponse { Success = false, Message = "User not found" });

        user.FirstName = request.FirstName ?? user.FirstName;
        user.LastName = request.LastName ?? user.LastName;
        user.Role = request.Role ?? user.Role;
        user.UpdatedAt = DateTime.UtcNow;

        await context.SaveChangesAsync();

        return Ok(new ApiResponse<UserDto>
        {
            Success = true,
            Message = "User updated successfully",
            Data = user.ToDto()
        });
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var user = await context.Users.FindAsync(id);
        if (user == null)
            return NotFound(new ApiResponse { Success = false, Message = "User not found" });

        context.Users.Remove(user);
        await context.SaveChangesAsync();

        return Ok(new ApiResponse
        {
            Success = true,
            Message = "User deleted successfully"
        });
    }

    [HttpPost("{id}/change-password")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> ChangePassword(int id, [FromBody] ChangePasswordRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(new ApiResponse { Success = false, Message = "Invalid input" });

        var user = await context.Users.FindAsync(id);
        if (user == null)
            return NotFound(new ApiResponse { Success = false, Message = "User not found" });

        if (request.NewPassword.Length < 6)
            return BadRequest(new ApiResponse { Success = false, Message = "Password must be at least 6 characters" });

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
        user.UpdatedAt = DateTime.UtcNow;

        await context.SaveChangesAsync();

        return Ok(new ApiResponse
        {
            Success = true,
            Message = "Password changed successfully"
        });
    }
}