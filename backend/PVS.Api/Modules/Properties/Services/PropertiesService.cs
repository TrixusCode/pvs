using PVS.Api.Data;
using PVS.Api.Models;
using PVS.Api.Modules.Properties.Dtos;

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
        var property = new Property
        {
            Title = request.Title,
            Description = request.Description,
            Price = request.Price,
            Bedrooms = request.Bedrooms,
            Bathrooms = request.Bathrooms,
            SquareFeet = request.SquareFeet,
            UserId = userId,
            CreatedAt = DateTime.UtcNow
        };

        context.Properties.Add(property);
        await context.SaveChangesAsync();

        return property;
    }

    public async Task<Property?> UpdateAsync(int id, UpdatePropertyRequest request, int userId)
    {
        var property = await GetByIdAsync(id);
        if (property == null || property.UserId != userId)
            return null;

        if (request.Title != null) property.Title = request.Title;
        if (request.Description != null) property.Description = request.Description;
        if (request.Price.HasValue) property.Price = request.Price.Value;
        if (request.Bedrooms.HasValue) property.Bedrooms = request.Bedrooms.Value;
        if (request.Bathrooms.HasValue) property.Bathrooms = request.Bathrooms.Value;
        if (request.SquareFeet.HasValue) property.SquareFeet = request.SquareFeet.Value;

        property.UpdatedAt = DateTime.UtcNow;

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