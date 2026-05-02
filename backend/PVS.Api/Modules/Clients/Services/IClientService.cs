using PVS.Api.Models;
using PVS.Api.Modules.Clients.Dtos;

namespace PVS.Api.Modules.Clients.Services;

public interface IClientService
{
    Task<IEnumerable<Client>> GetAllAsync(int userId);
    Task<Client?> GetByIdAsync(int id, int userId);
    Task<Client> CreateAsync(CreateClientRequest request, int userId);
    Task<Client?> UpdateAsync(int id, UpdateClientRequest request, int userId);
    Task<bool> DeleteAsync(int id, int userId);
}