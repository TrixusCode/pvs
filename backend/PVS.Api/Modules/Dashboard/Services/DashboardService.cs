using PVS.Api.Data;
using PVS.Api.Modules.Dashboard.Dtos;
using PVS.Api.Modules.Dashboard.Services;
using Microsoft.EntityFrameworkCore;

namespace PVS.Api.Modules.Dashboard.Services;

public class DashboardService(AppDbContext context) : IDashboardService
{
    public async Task<DashboardStatisticsDto> GetStatisticsAsync(int userId)
    {
        var totalProperties = await context.Properties.CountAsync(p => p.UserId == userId);
        var totalClients = await context.Clients.CountAsync(c => c.UserId == userId);
        var activeAppointments = await context.Appointments.CountAsync(a =>
            a.UserId == userId &&
            a.Status != PVS.Api.Modules.Appointments.Enums.AppointmentStatus.Cancelled &&
            a.Status != PVS.Api.Modules.Appointments.Enums.AppointmentStatus.NoShow &&
            a.AppointmentDate >= DateTime.UtcNow.Date);
        var pendingOffers = await context.Offers.CountAsync(o =>
            o.UserId == userId &&
            o.Status == PVS.Api.Modules.Offers.Enums.OfferStatus.Pending);

        return new DashboardStatisticsDto
        {
            TotalProperties = totalProperties,
            TotalClients = totalClients,
            ActiveAppointments = activeAppointments,
            PendingOffers = pendingOffers
        };
    }

    public async Task<List<ActivityDto>> GetRecentActivitiesAsync(int userId, int limit = 10)
    {
        var activities = new List<ActivityDto>();

        // Get recent property activities
        var recentProperties = await context.Properties
            .Where(p => p.UserId == userId)
            .OrderByDescending(p => p.CreatedAt)
            .Take(3)
            .ToListAsync();

        foreach (var property in recentProperties)
        {
            activities.Add(new ActivityDto
            {
                Type = "property",
                Description = $"Created property: {property.Title}",
                Timestamp = property.CreatedAt
            });
        }

        // Get recent client activities
        var recentClients = await context.Clients
            .Where(c => c.UserId == userId)
            .OrderByDescending(c => c.CreatedAt)
            .Take(3)
            .ToListAsync();

        foreach (var client in recentClients)
        {
            activities.Add(new ActivityDto
            {
                Type = "client",
                Description = $"Added client: {client.FirstName} {client.LastName}",
                Timestamp = client.CreatedAt
            });
        }

        // Get recent appointment activities
        var recentAppointments = await context.Appointments
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.CreatedAt)
            .Take(3)
            .ToListAsync();

        foreach (var appointment in recentAppointments)
        {
            activities.Add(new ActivityDto
            {
                Type = "appointment",
                Description = $"Scheduled appointment for {appointment.AppointmentDate.ToShortDateString()}",
                Timestamp = appointment.CreatedAt
            });
        }

        // Get recent offer activities
        var recentOffers = await context.Offers
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.CreatedAt)
            .Take(3)
            .ToListAsync();

        foreach (var offer in recentOffers)
        {
            activities.Add(new ActivityDto
            {
                Type = "offer",
                Description = $"Created offer for property ID: {offer.PropertyId}",
                Timestamp = offer.CreatedAt
            });
        }

        return activities
            .OrderByDescending(a => a.Timestamp)
            .Take(limit)
            .ToList();
    }

    public async Task<List<NotificationDto>> GetNotificationsAsync(int userId)
    {
        // For now, create some sample notifications based on upcoming appointments and pending offers
        var notifications = new List<NotificationDto>();

        // Check for upcoming appointments
        var upcomingAppointments = await context.Appointments
            .Where(a => a.UserId == userId &&
                       a.Status == PVS.Api.Modules.Appointments.Enums.AppointmentStatus.Scheduled &&
                       a.AppointmentDate >= DateTime.UtcNow.Date &&
                       a.AppointmentDate <= DateTime.UtcNow.Date.AddDays(7))
            .OrderBy(a => a.AppointmentDate)
            .Take(3)
            .ToListAsync();

        foreach (var appointment in upcomingAppointments)
        {
            notifications.Add(new NotificationDto
            {
                Id = appointment.Id * 1000, // Simple ID generation
                Message = $"Upcoming appointment on {appointment.AppointmentDate.ToShortDateString()} at {appointment.Time}",
                IsRead = false,
                CreatedAt = DateTime.UtcNow.AddDays(-1)
            });
        }

        // Check for pending offers
        var pendingOffers = await context.Offers
            .Where(o => o.UserId == userId &&
                       o.Status == PVS.Api.Modules.Offers.Enums.OfferStatus.Pending)
            .OrderByDescending(o => o.CreatedAt)
            .Take(2)
            .ToListAsync();

        foreach (var offer in pendingOffers)
        {
            notifications.Add(new NotificationDto
            {
                Id = offer.Id * 1001, // Simple ID generation
                Message = $"Pending offer for property ID: {offer.PropertyId} - ${offer.OfferedPrice:N0}",
                IsRead = false,
                CreatedAt = offer.CreatedAt
            });
        }

        return notifications
            .OrderByDescending(n => n.CreatedAt)
            .Take(10)
            .ToList();
    }

    public async Task<bool> MarkNotificationAsReadAsync(int notificationId, int userId)
    {
        // For now, this is a placeholder - in a real implementation,
        // you'd have a notifications table and mark the specific notification as read
        return true;
    }
}