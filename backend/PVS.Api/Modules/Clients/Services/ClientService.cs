using PVS.Api.Data;
using PVS.Api.Models;
using PVS.Api.Modules.Clients.Dtos;
using PVS.Api.Modules.Clients.Enums;

namespace PVS.Api.Modules.Clients.Services;

public class ClientService(AppDbContext context) : IClientService
{
    public async Task<IEnumerable<Client>> GetAllAsync(int userId)
    {
        return await Task.FromResult(context.Clients
            .Where(c => c.UserId == userId)
            .OrderByDescending(c => c.CreatedAt)
            .ToList());
    }

    public async Task<Client?> GetByIdAsync(int id, int userId)
    {
        return await Task.FromResult(context.Clients
            .FirstOrDefault(c => c.Id == id && c.UserId == userId));
    }

    public async Task<Client> CreateAsync(CreateClientRequest request, int userId)
    {
        var client = new Client
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Phone = request.Phone,
            Address = request.Address,
            ClientType = request.ClientType ?? ClientType.Buyer,
            Status = request.Status ?? ClientStatus.Active,
            UserId = userId,
            CreatedAt = DateTime.UtcNow
        };

        context.Clients.Add(client);
        await context.SaveChangesAsync();

        return client;
    }

    public async Task<Client?> UpdateAsync(int id, UpdateClientRequest request, int userId)
    {
        var client = await GetByIdAsync(id, userId);
        if (client == null)
            return null;

        if (request.FirstName != null) client.FirstName = request.FirstName;
        if (request.LastName != null) client.LastName = request.LastName;
        if (request.Email != null) client.Email = request.Email;
        if (request.Phone != null) client.Phone = request.Phone;
        if (request.Address != null) client.Address = request.Address;
        if (request.ClientType != null) client.ClientType = request.ClientType;
        if (request.Status != null) client.Status = request.Status;

        client.UpdatedAt = DateTime.UtcNow;

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