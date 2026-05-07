namespace PVS.Api.Modules.Appointments.Dtos;

using PVS.Api.Modules.Appointments.Enums;

public class AppointmentDto
{
    public int Id { get; set; }
    public int PropertyId { get; set; }
    public int ClientId { get; set; }
    public DateTime AppointmentDate { get; set; }
    public string Time { get; set; } = string.Empty;
    public AppointmentType? Type { get; set; }
    public AppointmentStatus? Status { get; set; }
    public string Notes { get; set; } = string.Empty;
    public decimal? DurationMinutes { get; set; }
    public string? FeedbackFromClient { get; set; }
    public int UserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
