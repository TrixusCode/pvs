using PVS.Api.Models;
using PVS.Api.Modules.Offers.Dtos;
using PVS.Api.Modules.Offers.Repository;
using PVS.Api.Modules.Offers.Enums;
using PVS.Api.Modules.Offers.Mappers;

namespace PVS.Api.Modules.Offers.Services;

public class OfferService : IOfferService
{
    private readonly IOfferRepository _offerRepository;

    public OfferService(IOfferRepository offerRepository)
    {
        _offerRepository = offerRepository;
    }

    public async Task<IEnumerable<Offer>> GetAllByUserAsync(int userId)
    {
        return await _offerRepository.GetByUserIdAsync(userId);
    }

    public async Task<Offer?> GetByIdAsync(int id)
    {
        return await _offerRepository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<Offer>> GetByPropertyAsync(int propertyId)
    {
        return await _offerRepository.GetByPropertyIdAsync(propertyId);
    }

    public async Task<IEnumerable<Offer>> GetByClientAsync(int clientId)
    {
        return await _offerRepository.GetByClientIdAsync(clientId);
    }

    public async Task<IEnumerable<Offer>> GetByStatusAsync(OfferStatus status)
    {
        return await _offerRepository.GetByStatusAsync(status);
    }

    public async Task<Offer> CreateAsync(CreateOfferRequest request, int userId)
    {
        var offer = request.ToEntity(userId);

        await _offerRepository.AddAsync(offer);
        return offer;
    }

    public async Task<Offer?> UpdateAsync(int id, UpdateOfferRequest request, int userId)
    {
        var offer = await _offerRepository.GetByIdAsync(id);
        if (offer == null)
            return null;

        request.ApplyTo(offer);

        await _offerRepository.UpdateAsync(offer);
        return offer;
    }

    public async Task<bool> DeleteAsync(int id, int userId)
    {
        var offer = await _offerRepository.GetByIdAsync(id);
        if (offer == null)
            return false;

        await _offerRepository.DeleteAsync(offer);
        return true;
    }
}
