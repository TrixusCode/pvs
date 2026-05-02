using PVS.Api.Common.Repository;
using PVS.Api.Data;
using PVS.Api.Models;
using PVS.Api.Modules.Clients.Enums;

namespace PVS.Api.Modules.Clients.Repository;

public interface IClientRepository : IGenericRepository<Client>
{
    Task<IEnumerable<Client>> GetByUserIdAsync(int userId);
    Task<IEnumerable<Client>> GetByStatusAsync(ClientStatus status);
    Task<IEnumerable<Client>> GetByTypeAsync(ClientType type);
    Task<Client?> GetByEmailAsync(string email);
}

public class ClientRepository : GenericRepository<Client>, IClientRepository
{
    private readonly AppDbContext _context;

    public ClientRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Client>> GetByUserIdAsync(int userId)
    {
        return await Task.FromResult(_context.Clients
            .Where(c => c.UserId == userId)
            .OrderByDescending(c => c.CreatedAt)
            .ToList());
    }

    public async Task<IEnumerable<Client>> GetByStatusAsync(ClientStatus status)
    {
        return await Task.FromResult(_context.Clients
            .Where(c => c.Status == status)
            .ToList());
    }

    public async Task<IEnumerable<Client>> GetByTypeAsync(ClientType type)
    {
        return await Task.FromResult(_context.Clients
            .Where(c => c.ClientType == type)
            .ToList());
    }

    public async Task<Client?> GetByEmailAsync(string email)
    {
        return await Task.FromResult(_context.Clients
            .FirstOrDefault(c => c.Email.Equals(email, StringComparison.OrdinalIgnoreCase)));
    }
}