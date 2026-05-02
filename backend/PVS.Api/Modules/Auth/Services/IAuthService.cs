using PVS.Api.Models;
using PVS.Api.Modules.Auth.Dtos;

namespace PVS.Api.Modules.Auth.Services;

public interface IAuthService
{
    Task<User?> AuthenticateAsync(string email, string password);
    Task<User?> GetUserByIdAsync(int id);
    Task<User?> GetUserByEmailAsync(string email);
    Task<User> RegisterAsync(RegisterRequest request);
    Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword);
}