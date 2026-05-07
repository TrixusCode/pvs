using PVS.Api.Modules.Offers.Enums;

namespace PVS.Api.Modules.Offers.Dtos;

public class UpdateOfferRequest
{
    public decimal OfferedPrice { get; set; }
    public string? Status { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public decimal? DownPaymentPercent { get; set; }
    public int? ClosingDaysRequested { get; set; }
    public string? Contingencies { get; set; }
    public string? AgentNotes { get; set; }
}