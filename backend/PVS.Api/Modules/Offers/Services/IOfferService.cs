using PVS.Api.Models;
using PVS.Api.Modules.Offers.Dtos;
using PVS.Api.Modules.Offers.Enums;

namespace PVS.Api.Modules.Offers.Services;

public interface IOfferService
{
    Task<IEnumerable<Offer>> GetAllByUserAsync(int userId);
    Task<Offer?> GetByIdAsync(int id);
    Task<IEnumerable<Offer>> GetByPropertyAsync(int propertyId);
    Task<IEnumerable<Offer>> GetByClientAsync(int clientId);
    Task<IEnumerable<Offer>> GetByStatusAsync(OfferStatus status);
    Task<Offer> CreateAsync(CreateOfferRequest request, int userId);
    Task<Offer?> UpdateAsync(int id, UpdateOfferRequest request, int userId);
    Task<bool> DeleteAsync(int id, int userId);
}