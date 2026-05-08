using PVS.Api.Modules.Offers.Enums;

namespace PVS.Api.Modules.Offers.Dtos;

public class UpdateOfferRequest
{
    public int? PropertyId { get; set; }
    public int? ClientId { get; set; }
    public decimal OfferedPrice { get; set; }
    public string? Status { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public OfferType? OfferType { get; set; }
    public decimal? DownPaymentPercent { get; set; }
    public int? ClosingDaysRequested { get; set; }
    public string? Contingencies { get; set; }
    public string? AgentNotes { get; set; }
}
