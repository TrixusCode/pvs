using PVS.Api.Data;
using PVS.Api.Models;
using PVS.Api.Modules.Clients.Dtos;
using PVS.Api.Modules.Clients.Mappers;
using Microsoft.EntityFrameworkCore;

namespace PVS.Api.Modules.Clients.Services;

public class ClientService(AppDbContext context) : IClientService
{
    public async Task<IEnumerable<Client>> GetAllAsync(int userId)
    {
        return await Task.FromResult(context.Clients
            .Include(c => c.Address)
            .Where(c => c.UserId == userId)
            .OrderByDescending(c => c.CreatedAt)
            .ToList());
    }

    public async Task<Client?> GetByIdAsync(int id, int userId)
    {
        return await Task.FromResult(context.Clients
            .Include(c => c.Address)
            .FirstOrDefault(c => c.Id == id && c.UserId == userId));
    }

    public async Task<Client> CreateAsync(CreateClientRequest request, int userId)
    {
        var client = request.ToEntity(userId);

        context.Clients.Add(client);
        await context.SaveChangesAsync();

        return client;
    }

    public async Task<Client?> UpdateAsync(int id, UpdateClientRequest request, int userId)
    {
        var client = await GetByIdAsync(id, userId);
        if (client == null)
            return null;

        request.ApplyTo(client);

        await context.SaveChangesAsync();
        return client;
    }

    public async Task<bool> DeleteAsync(int id, int userId)
    {
        var client = await GetByIdAsync(id, userId);
        if (client == null)
            return false;

        context.Clients.Remove(client);
        await context.SaveChangesAsync();
        return true;
    }
}
