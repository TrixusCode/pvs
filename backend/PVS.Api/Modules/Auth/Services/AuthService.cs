using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using PVS.Api.Data;
using PVS.Api.Models;
using PVS.Api.Modules.Auth.Dtos;

namespace PVS.Api.Modules.Auth.Services;

public class AuthService(AppDbContext context) : IAuthService
{
    public async Task<User?> AuthenticateAsync(string email, string password)
    {
        var user = await GetUserByEmailAsync(email);
        if (user == null)
            return null;

        if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            return null;

        return user;
    }

    public async Task<User?> GetUserByIdAsync(int id)
    {
        return await context.Users.FindAsync(id);
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        var user = await context.Users
            .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
        return user;
    }

    public async Task<User> RegisterAsync(RegisterRequest request)
    {
        // Check if user already exists
        if (await GetUserByEmailAsync(request.Email) != null)
            throw new InvalidOperationException("Email already registered");

        var user = new User
        {
            Email = request.Email.ToLower().Trim(),
            FirstName = request.FirstName.Trim(),
            LastName = request.LastName.Trim(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = request.Role ?? "User",
            CreatedAt = DateTime.UtcNow
        };

        context.Users.Add(user);
        await context.SaveChangesAsync();

        return user;
    }

    public async Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
    {
        var user = await GetUserByIdAsync(userId);
        if (user == null)
            return false;

        if (!BCrypt.Net.BCrypt.Verify(currentPassword, user.PasswordHash))
            return false;

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
        user.UpdatedAt = DateTime.UtcNow;

        await context.SaveChangesAsync();
        return true;
    }
}