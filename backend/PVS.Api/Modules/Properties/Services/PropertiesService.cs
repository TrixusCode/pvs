using PVS.Api.Data;
using PVS.Api.Models;
using PVS.Api.Modules.Properties.Dtos;
using PVS.Api.Modules.Properties.Mappers;

namespace PVS.Api.Modules.Properties.Services;

public class PropertiesService(AppDbContext context) : IPropertiesService
{
    public async Task<IEnumerable<Property>> GetAllAsync(int page = 1, int pageSize = 10)
    {
        var skip = (page - 1) * pageSize;
        return await Task.FromResult(context.Properties
            .OrderByDescending(p => p.CreatedAt)
            .Skip(skip)
            .Take(pageSize)
            .ToList());
    }

    public async Task<Property?> GetByIdAsync(int id)
    {
        return await context.Properties.FindAsync(id);
    }

    public async Task<Property> CreateAsync(CreatePropertyRequest request, int userId)
    {
        var property = request.ToEntity(userId);

        context.Properties.Add(property);
        await context.SaveChangesAsync();

        return property;
    }

    public async Task<Property?> UpdateAsync(int id, UpdatePropertyRequest request, int userId)
    {
        var property = await GetByIdAsync(id);
        if (property == null || property.UserId != userId)
            return null;

        request.ApplyTo(property);

        await context.SaveChangesAsync();
        return property;
    }

    public async Task<bool> DeleteAsync(int id, int userId)
    {
        var property = await GetByIdAsync(id);
        if (property == null || property.UserId != userId)
            return false;

        context.Properties.Remove(property);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<int> GetTotalCountAsync()
    {
        return await Task.FromResult(context.Properties.Count());
    }
}
