using PVS.Api.Modules.Offers.Enums;

namespace PVS.Api.Models;

public class Offer
{
    public int Id { get; set; }
    public int PropertyId { get; set; }
    public int ClientId { get; set; }
    public decimal OfferedPrice { get; set; }
    public OfferStatus? Status { get; set; } = OfferStatus.Pending; 
    public DateTime OfferDate { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public OfferType OfferType { get; set; } = OfferType.FullPrice; 
    public decimal? DownPaymentPercent { get; set; }
    public int? ClosingDaysRequested { get; set; }
    public Contingencies? Contingencies { get; set; } 
    public string? AgentNotes { get; set; }
    public int UserId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}