using PVS.Api.Common.Repository;
using PVS.Api.Data;
using PVS.Api.Models;
using PVS.Api.Modules.Properties.Enums;

namespace PVS.Api.Modules.Properties.Repository;

public interface IPropertyRepository : IGenericRepository<Property>
{
    Task<IEnumerable<Property>> GetByUserIdAsync(int userId);
    Task<IEnumerable<Property>> GetByPriceRangeAsync(decimal minPrice, decimal maxPrice);
    Task<IEnumerable<Property>> GetByTypeAsync(PropertyType type);
    Task<IEnumerable<Property>> GetByBedroomsAsync(int bedrooms);
}

public class PropertyRepository : GenericRepository<Property>, IPropertyRepository
{
    private readonly AppDbContext _context;

    public PropertyRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Property>> GetByUserIdAsync(int userId)
    {
        return await Task.FromResult(_context.Properties
            .Where(p => p.UserId == userId)
            .OrderByDescending(p => p.CreatedAt)
            .ToList());
    }

    public async Task<IEnumerable<Property>> GetByPriceRangeAsync(decimal minPrice, decimal maxPrice)
    {
        return await Task.FromResult(_context.Properties
            .Where(p => p.Price >= minPrice && p.Price <= maxPrice)
            .OrderBy(p => p.Price)
            .ToList());
    }

    public async Task<IEnumerable<Property>> GetByTypeAsync(PropertyType type)
    {
        return await Task.FromResult(_context.Properties
            .Where(p => p.PropertyType == type)
            .ToList());
    }

    public async Task<IEnumerable<Property>> GetByBedroomsAsync(int bedrooms)
    {
        return await Task.FromResult(_context.Properties
            .Where(p => p.Bedrooms == bedrooms)
            .ToList());
    }
}