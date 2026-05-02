using PVS.Api.Modules.Appointments.Enums;

namespace PVS.Api.Models;

public class Appointment
{
    public int Id { get; set; }
    public int PropertyId { get; set; }
    public int ClientId { get; set; }
    public DateTime AppointmentDate { get; set; }
    public string Time { get; set; } = string.Empty; // e.g., "10:00 AM"
    public AppointmentType? Type { get; set; }  // Showing, Inspection, Closing
    public AppointmentStatus? Status { get; set; }  // Scheduled, Completed, Cancelled, No-Show
    public string Notes { get; set; } = string.Empty;
    public decimal? DurationMinutes { get; set; }
    public string? FeedbackFromClient { get; set; }
    public int UserId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}