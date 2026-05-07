using PVS.Api.Common.Repository;
using PVS.Api.Data;
using PVS.Api.Models;
using PVS.Api.Modules.Offers.Enums;

namespace PVS.Api.Modules.Offers.Repository;

public class OfferRepository(AppDbContext context) : GenericRepository<Offer>(context), IOfferRepository
{
    private readonly AppDbContext _context = context;

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