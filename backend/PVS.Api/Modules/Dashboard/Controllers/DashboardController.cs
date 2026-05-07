using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PVS.Api.Common;
using PVS.Api.Modules.Dashboard.Dtos;
using PVS.Api.Modules.Dashboard.Services;
using System.Security.Claims;

namespace PVS.Api.Modules.Dashboard.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _dashboardService;

    public DashboardController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(userIdClaim, out var userId) ? userId : 0;
    }

    [HttpGet("statistics")]
    public async Task<IActionResult> GetStatistics()
    {
        var userId = GetCurrentUserId();
        if (userId == 0)
            return Unauthorized(new ApiResponse { Success = false, Message = "User not authenticated" });

        var statistics = await _dashboardService.GetStatisticsAsync(userId);

        return Ok(new ApiResponse<DashboardStatisticsDto>
        {
            Success = true,
            Data = statistics
        });
    }

    [HttpGet("activities")]
    public async Task<IActionResult> GetRecentActivities([FromQuery] int limit = 10)
    {
        var userId = GetCurrentUserId();
        if (userId == 0)
            return Unauthorized(new ApiResponse { Success = false, Message = "User not authenticated" });

        var activities = await _dashboardService.GetRecentActivitiesAsync(userId, limit);

        return Ok(new ApiResponse<List<ActivityDto>>
        {
            Success = true,
            Data = activities
        });
    }

    [HttpGet("notifications")]
    public async Task<IActionResult> GetNotifications()
    {
        var userId = GetCurrentUserId();
        if (userId == 0)
            return Unauthorized(new ApiResponse { Success = false, Message = "User not authenticated" });

        var notifications = await _dashboardService.GetNotificationsAsync(userId);

        return Ok(new ApiResponse<List<NotificationDto>>
        {
            Success = true,
            Data = notifications
        });
    }

    [HttpPost("notifications/{id}/read")]
    public async Task<IActionResult> MarkNotificationAsRead(int id)
    {
        var userId = GetCurrentUserId();
        if (userId == 0)
            return Unauthorized(new ApiResponse { Success = false, Message = "User not authenticated" });

        var success = await _dashboardService.MarkNotificationAsReadAsync(id, userId);

        if (!success)
            return NotFound(new ApiResponse { Success = false, Message = "Notification not found" });

        return Ok(new ApiResponse
        {
            Success = true,
            Message = "Notification marked as read"
        });
    }
}