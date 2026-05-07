using PVS.Api.Models;
using PVS.Api.Modules.Offers.Dtos;
using PVS.Api.Modules.Offers.Enums;

namespace PVS.Api.Modules.Offers.Mappers;

public static class OfferMapper
{
    public static OfferDto ToDto(this Offer offer)
    {
        return new OfferDto
        {
            Id = offer.Id,
            PropertyId = offer.PropertyId,
            ClientId = offer.ClientId,
            OfferedPrice = offer.OfferedPrice,
            Status = offer.Status.ToString(),
            OfferDate = offer.OfferDate,
            ExpirationDate = offer.ExpirationDate,
            OfferType = offer.OfferType.ToString(),
            DownPaymentPercent = offer.DownPaymentPercent,
            ClosingDaysRequested = offer.ClosingDaysRequested,
            Contingencies = offer.Contingencies.ToString(),
            AgentNotes = offer.AgentNotes,
            UserId = offer.UserId,
            CreatedAt = offer.CreatedAt,
            UpdatedAt = offer.UpdatedAt
        };
    }

    public static Offer ToEntity(this CreateOfferRequest request, int userId)
    {
        return new Offer
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
    }

    public static void ApplyTo(this UpdateOfferRequest request, Offer offer)
    {
        if (request.OfferedPrice > 0) offer.OfferedPrice = request.OfferedPrice;
        if (!string.IsNullOrEmpty(request.Status) && Enum.TryParse<OfferStatus>(request.Status, out var status)) 
            offer.Status = status;
        if (request.ExpirationDate.HasValue) offer.ExpirationDate = request.ExpirationDate;
        if (request.DownPaymentPercent.HasValue) offer.DownPaymentPercent = request.DownPaymentPercent;
        if (request.ClosingDaysRequested.HasValue) offer.ClosingDaysRequested = request.ClosingDaysRequested;
        if (!string.IsNullOrEmpty(request.Contingencies) && Enum.TryParse<Contingencies>(request.Contingencies, out var contingencies)) 
            offer.Contingencies = contingencies;
        if (!string.IsNullOrEmpty(request.AgentNotes)) offer.AgentNotes = request.AgentNotes;

        offer.UpdatedAt = DateTime.UtcNow;
    }
}
