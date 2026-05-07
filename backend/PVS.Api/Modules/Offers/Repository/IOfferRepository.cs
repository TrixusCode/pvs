using PVS.Api.Common.Repository;
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