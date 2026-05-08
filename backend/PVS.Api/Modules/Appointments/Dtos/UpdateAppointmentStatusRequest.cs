using PVS.Api.Modules.Appointments.Enums;

namespace PVS.Api.Modules.Appointments.Dtos;

public class UpdateAppointmentStatusRequest
{
    public AppointmentStatus Status { get; set; }
}
