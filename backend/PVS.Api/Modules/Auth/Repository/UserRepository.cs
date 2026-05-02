using Microsoft.EntityFrameworkCore;
using PVS.Api.Common.Repository;
using PVS.Api.Data;
using PVS.Api.Models;

namespace PVS.Api.Modules.Auth.Repository;

public interface IUserRepository : IGenericRepository<User>
{
    Task<User?> GetByEmailAsync(string email);
}

public class UserRepository(AppDbContext context) : GenericRepository<User>(context), IUserRepository
{
    private readonly AppDbContext _context = context;

    public async Task<User?> GetByEmailAsync(string email)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
        return user;
    }
}
