using PVS.Api.Models;
using PVS.Api.Modules.Offers.Dtos;
using PVS.Api.Modules.Offers.Repository;
using PVS.Api.Modules.Offers.Enums;

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
        var offer = new Offer
        {
            PropertyId = request.PropertyId,
            ClientId = request.ClientId,
            OfferedPrice = request.OfferedPrice,
            Status = request.Status ?? OfferStatus.Pending,
            OfferDate = DateTime.UtcNow,
            ExpirationDate = request.ExpirationDate,
            OfferType = request.OfferType ?? OfferType.FullPrice,
            DownPaymentPercent = request.DownPaymentPercent,
            ClosingDaysRequested = request.ClosingDaysRequested,
            Contingencies = request.Contingencies,
            AgentNotes = request.AgentNotes,
            UserId = userId,
            CreatedAt = DateTime.UtcNow
        };

        await _offerRepository.AddAsync(offer);
        return offer;
    }

    public async Task<Offer?> UpdateAsync(int id, UpdateOfferRequest request, int userId)
    {
        var offer = await _offerRepository.GetByIdAsync(id);
        if (offer == null)
            return null;

        if (request.OfferedPrice > 0) offer.OfferedPrice = request.OfferedPrice;
        if (request.Status.HasValue) offer.Status = request.Status;
        if (request.ExpirationDate.HasValue) offer.ExpirationDate = request.ExpirationDate;
        if (request.DownPaymentPercent.HasValue) offer.DownPaymentPercent = request.DownPaymentPercent;
        if (request.ClosingDaysRequested.HasValue) offer.ClosingDaysRequested = request.ClosingDaysRequested;
        if (request.Contingencies.HasValue) offer.Contingencies = request.Contingencies;
        if (!string.IsNullOrEmpty(request.AgentNotes)) offer.AgentNotes = request.AgentNotes;
        offer.UpdatedAt = DateTime.UtcNow;

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