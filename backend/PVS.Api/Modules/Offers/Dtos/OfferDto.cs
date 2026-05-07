namespace PVS.Api.Modules.Offers.Dtos;


public class OfferDto
{
    public int Id { get; set; }
    public int PropertyId { get; set; }
    public int ClientId { get; set; }
    public decimal OfferedPrice { get; set; }
    public string? Status { get; set; }
    public DateTime OfferDate { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public string OfferType { get; set; }
    public decimal? DownPaymentPercent { get; set; }
    public int? ClosingDaysRequested { get; set; }
    public string? Contingencies { get; set; }
    public string? AgentNotes { get; set; }
    public int UserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
