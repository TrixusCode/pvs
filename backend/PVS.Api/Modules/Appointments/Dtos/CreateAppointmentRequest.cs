using PVS.Api.Modules.Appointments.Enums;

namespace PVS.Api.Modules.Appointments.Dtos;

public class CreateAppointmentRequest
{
    public int PropertyId { get; set; }
    public int ClientId { get; set; }
    public DateTime AppointmentDate { get; set; }
    public string Time { get; set; } = string.Empty;
    public AppointmentType? Type { get; set; }
    public AppointmentStatus? Status { get; set; }
    public string? Notes { get; set; }
    public decimal? DurationMinutes { get; set; }
}