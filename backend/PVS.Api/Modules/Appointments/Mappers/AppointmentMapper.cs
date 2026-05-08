using PVS.Api.Models;
using PVS.Api.Modules.Appointments.Dtos;
using PVS.Api.Modules.Appointments.Enums;

namespace PVS.Api.Modules.Appointments.Mappers;

public static class AppointmentMapper
{
    public static AppointmentDto ToDto(this Appointment appointment)
    {
        return new AppointmentDto
        {
            Id = appointment.Id,
            PropertyId = appointment.PropertyId,
            ClientId = appointment.ClientId,
            AppointmentDate = appointment.AppointmentDate,
            Time = appointment.Time,
            Type = appointment.Type,
            Status = appointment.Status,
            Notes = appointment.Notes,
            DurationMinutes = appointment.DurationMinutes,
            FeedbackFromClient = appointment.FeedbackFromClient,
            UserId = appointment.UserId,
            CreatedAt = appointment.CreatedAt,
            UpdatedAt = appointment.UpdatedAt
        };
    }

    public static Appointment ToEntity(this CreateAppointmentRequest request, int userId)
    {
        return new Appointment
        {
            PropertyId = request.PropertyId,
            ClientId = request.ClientId,
            AppointmentDate = request.AppointmentDate,
            Time = request.Time,
            Type = request.Type ?? AppointmentType.Showing,
            Status = request.Status ?? AppointmentStatus.Scheduled,
            Notes = request.Notes ?? string.Empty,
            DurationMinutes = request.DurationMinutes,
            UserId = userId,
            CreatedAt = DateTime.UtcNow
        };
    }

    public static void ApplyTo(this UpdateAppointmentRequest request, Appointment appointment)
    {
        if (request.PropertyId.HasValue) appointment.PropertyId = request.PropertyId.Value;
        if (request.ClientId.HasValue) appointment.ClientId = request.ClientId.Value;
        if (request.AppointmentDate != default) appointment.AppointmentDate = request.AppointmentDate;
        if (!string.IsNullOrEmpty(request.Time)) appointment.Time = request.Time;
        if (request.Type.HasValue) appointment.Type = request.Type;
        if (request.Status.HasValue) appointment.Status = request.Status;
        if (!string.IsNullOrEmpty(request.Notes)) appointment.Notes = request.Notes;
        if (request.DurationMinutes.HasValue) appointment.DurationMinutes = request.DurationMinutes;
        if (!string.IsNullOrEmpty(request.FeedbackFromClient)) appointment.FeedbackFromClient = request.FeedbackFromClient;

        appointment.UpdatedAt = DateTime.UtcNow;
    }
}
