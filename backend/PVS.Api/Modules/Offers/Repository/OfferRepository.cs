using PVS.Api.Common.Repository;
using PVS.Api.Data;
using PVS.Api.Models;
using PVS.Api.Modules.Offers.Enums;

namespace PVS.Api.Modules.Offers.Repository;

public interface IOfferRepository : IGenericRepository<Offer>
{
    Task<IEnumerable<Offer>> GetByPropertyIdAsync(int propertyId);
    Task<IEnumerable<Offer>> GetByClientIdAsync(int clientId);
    Task<IEnumerable<Offer>> GetByUserIdAsync(int userId);
    Task<IEnumerable<Offer>> GetByStatusAsync(OfferStatus status);
    Task<IEnumerable<Offer>> GetExpiredAsync();
}

public class OfferRepository : GenericRepository<Offer>, IOfferRepository
{
    private readonly AppDbContext _context;

    public OfferRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Offer>> GetByPropertyIdAsync(int propertyId)
    {
        return await Task.FromResult(_context.Offers
            .Where(o => o.PropertyId == propertyId)
            .OrderByDescending(o => o.OfferDate)
            .ToList());
    }

    public async Task<IEnumerable<Offer>> GetByClientIdAsync(int clientId)
    {
        return await Task.FromResult(_context.Offers
            .Where(o => o.ClientId == clientId)
            .OrderByDescending(o => o.OfferDate)
            .ToList());
    }

    public async Task<IEnumerable<Offer>> GetByUserIdAsync(int userId)
    {
        return await Task.FromResult(_context.Offers
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.OfferDate)
            .ToList());
    }

    public async Task<IEnumerable<Offer>> GetByStatusAsync(OfferStatus status)
    {
        return await Task.FromResult(_context.Offers
            .Where(o => o.Status == status)
            .OrderByDescending(o => o.OfferDate)
            .ToList());
    }

    public async Task<IEnumerable<Offer>> GetExpiredAsync()
    {
        return await Task.FromResult(_context.Offers
            .Where(o => o.ExpirationDate.HasValue && o.ExpirationDate < DateTime.UtcNow)
            .ToList());
    }
}