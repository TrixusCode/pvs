using PVS.Api.Modules.Appointments.Enums;

namespace PVS.Api.Modules.Appointments.Dtos;

public class UpdateAppointmentRequest
{
    public DateTime AppointmentDate { get; set; }
    public string? Time { get; set; }
    public AppointmentType? Type { get; set; }
    public AppointmentStatus? Status { get; set; }
    public string? Notes { get; set; }
    public decimal? DurationMinutes { get; set; }
    public string? FeedbackFromClient { get; set; }
}