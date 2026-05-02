using PVS.Api.Models;
using PVS.Api.Modules.Appointments.Dtos;
using PVS.Api.Modules.Appointments.Repository;
using PVS.Api.Modules.Appointments.Enums;

namespace PVS.Api.Modules.Appointments.Services;

public class AppointmentService : IAppointmentService
{
    private readonly IAppointmentRepository _appointmentRepository;

    public AppointmentService(IAppointmentRepository appointmentRepository)
    {
        _appointmentRepository = appointmentRepository;
    }

    public async Task<IEnumerable<Appointment>> GetAllByUserAsync(int userId)
    {
        return await _appointmentRepository.GetByUserIdAsync(userId);
    }

    public async Task<Appointment?> GetByIdAsync(int id)
    {
        return await _appointmentRepository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<Appointment>> GetByPropertyAsync(int propertyId)
    {
        return await _appointmentRepository.GetByPropertyIdAsync(propertyId);
    }

    public async Task<IEnumerable<Appointment>> GetByClientAsync(int clientId)
    {
        return await _appointmentRepository.GetByClientIdAsync(clientId);
    }

    public async Task<IEnumerable<Appointment>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _appointmentRepository.GetByDateRangeAsync(startDate, endDate);
    }

    public async Task<Appointment> CreateAsync(CreateAppointmentRequest request, int userId)
    {
        var appointment = new Appointment
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

        await _appointmentRepository.AddAsync(appointment);
        return appointment;
    }

    public async Task<Appointment?> UpdateAsync(int id, UpdateAppointmentRequest request, int userId)
    {
        var appointment = await _appointmentRepository.GetByIdAsync(id);
        if (appointment == null)
            return null;

        if (request.AppointmentDate != default) appointment.AppointmentDate = request.AppointmentDate;
        if (!string.IsNullOrEmpty(request.Time)) appointment.Time = request.Time;
        if (request.Type.HasValue) appointment.Type = request.Type;
        if (request.Status.HasValue) appointment.Status = request.Status;
        if (!string.IsNullOrEmpty(request.Notes)) appointment.Notes = request.Notes;
        if (request.DurationMinutes.HasValue) appointment.DurationMinutes = request.DurationMinutes;
        if (!string.IsNullOrEmpty(request.FeedbackFromClient)) appointment.FeedbackFromClient = request.FeedbackFromClient;
        appointment.UpdatedAt = DateTime.UtcNow;

        await _appointmentRepository.UpdateAsync(appointment);
        return appointment;
    }

    public async Task<bool> DeleteAsync(int id, int userId)
    {
        var appointment = await _appointmentRepository.GetByIdAsync(id);
        if (appointment == null)
            return false;

        await _appointmentRepository.DeleteAsync(appointment);
        return true;
    }
}