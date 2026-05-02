using PVS.Api.Models;
using PVS.Api.Modules.Appointments.Dtos;

namespace PVS.Api.Modules.Appointments.Services;

public interface IAppointmentService
{
    Task<IEnumerable<Appointment>> GetAllByUserAsync(int userId);
    Task<Appointment?> GetByIdAsync(int id);
    Task<IEnumerable<Appointment>> GetByPropertyAsync(int propertyId);
    Task<IEnumerable<Appointment>> GetByClientAsync(int clientId);
    Task<IEnumerable<Appointment>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<Appointment> CreateAsync(CreateAppointmentRequest request, int userId);
    Task<Appointment?> UpdateAsync(int id, UpdateAppointmentRequest request, int userId);
    Task<bool> DeleteAsync(int id, int userId);
}