using PVS.Api.Common.Repository;
using PVS.Api.Data;
using PVS.Api.Models;
using PVS.Api.Modules.Appointments.Enums;

namespace PVS.Api.Modules.Appointments.Repository;

public interface IAppointmentRepository : IGenericRepository<Appointment>
{
    Task<IEnumerable<Appointment>> GetByPropertyIdAsync(int propertyId);
    Task<IEnumerable<Appointment>> GetByClientIdAsync(int clientId);
    Task<IEnumerable<Appointment>> GetByUserIdAsync(int userId);
    Task<IEnumerable<Appointment>> GetByStatusAsync(AppointmentStatus status);
    Task<IEnumerable<Appointment>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
}

public class AppointmentRepository : GenericRepository<Appointment>, IAppointmentRepository
{
    private readonly AppDbContext _context;

    public AppointmentRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Appointment>> GetByPropertyIdAsync(int propertyId)
    {
        return await Task.FromResult(_context.Appointments
            .Where(a => a.PropertyId == propertyId)
            .OrderByDescending(a => a.AppointmentDate)
            .ToList());
    }

    public async Task<IEnumerable<Appointment>> GetByClientIdAsync(int clientId)
    {
        return await Task.FromResult(_context.Appointments
            .Where(a => a.ClientId == clientId)
            .OrderByDescending(a => a.AppointmentDate)
            .ToList());
    }

    public async Task<IEnumerable<Appointment>> GetByUserIdAsync(int userId)
    {
        return await Task.FromResult(_context.Appointments
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.AppointmentDate)
            .ToList());
    }

    public async Task<IEnumerable<Appointment>> GetByStatusAsync(AppointmentStatus status)
    {
        return await Task.FromResult(_context.Appointments
            .Where(a => a.Status == status)
            .OrderBy(a => a.AppointmentDate)
            .ToList());
    }

    public async Task<IEnumerable<Appointment>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await Task.FromResult(_context.Appointments
            .Where(a => a.AppointmentDate >= startDate && a.AppointmentDate <= endDate)
            .OrderBy(a => a.AppointmentDate)
            .ToList());
    }
}