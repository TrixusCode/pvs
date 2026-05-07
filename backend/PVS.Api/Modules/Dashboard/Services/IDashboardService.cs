using PVS.Api.Modules.Dashboard.Dtos;

namespace PVS.Api.Modules.Dashboard.Services;

public interface IDashboardService
{
    Task<DashboardStatisticsDto> GetStatisticsAsync(int userId);
    Task<List<ActivityDto>> GetRecentActivitiesAsync(int userId, int limit = 10);
    Task<List<NotificationDto>> GetNotificationsAsync(int userId);
    Task<bool> MarkNotificationAsReadAsync(int notificationId, int userId);
}