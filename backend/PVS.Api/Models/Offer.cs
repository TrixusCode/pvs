namespace PVS.Api.Models;

public class Offer
{
    public int Id { get; set; }
    public int PropertyId { get; set; }
    public int ClientId { get; set; }
    public decimal OfferedPrice { get; set; }
    public string Status { get; set; } = "Pending"; // Pending, Accepted, Rejected, Withdrawn, Expired
    public DateTime OfferDate { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public string OfferType { get; set; } = "Full Price"; // Full Price, Contingent, As-Is
    public decimal? DownPaymentPercent { get; set; }
    public int? ClosingDaysRequested { get; set; }
    public string? Contingencies { get; set; } // Home inspection, appraisal, financing, etc.
    public string? AgentNotes { get; set; }
    public int UserId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}