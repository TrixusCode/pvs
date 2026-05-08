using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PVS.Api.Common;
using PVS.Api.Modules.Appointments.Dtos;
using PVS.Api.Modules.Appointments.Enums;
using PVS.Api.Modules.Appointments.Mappers;
using PVS.Api.Modules.Appointments.Services;
using System.Security.Claims;

namespace PVS.Api.Modules.Appointments.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AppointmentsController(IAppointmentService appointmentService) : ControllerBase
{
    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(userIdClaim, out var userId) ? userId : 0;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var userId = GetCurrentUserId();
        var appointments = await appointmentService.GetAllByUserAsync(userId);

        var skip = (page - 1) * pageSize;
        var total = appointments.Count();
        var items = appointments.Skip(skip).Take(pageSize).ToList();

        return Ok(new PaginatedResponse<AppointmentDto>
        {
            Data = items.Select(appointment => appointment.ToDto()).ToList(),
            CurrentPage = page,
            PageSize = pageSize,
            TotalCount = total
        });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var appointment = await appointmentService.GetByIdAsync(id);
        if (appointment == null)
            return NotFound(new ApiResponse { Success = false, Message = "Appointment not found" });

        return Ok(new ApiResponse<AppointmentDto>
        {
            Success = true,
            Data = appointment.ToDto()
        });
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string? query)
    {
        var userId = GetCurrentUserId();
        var appointments = await appointmentService.GetAllByUserAsync(userId);

        if (!string.IsNullOrWhiteSpace(query))
        {
            var normalizedQuery = query.Trim();
            appointments = appointments.Where(appointment =>
                appointment.PropertyId.ToString().Contains(normalizedQuery, StringComparison.OrdinalIgnoreCase) ||
                appointment.ClientId.ToString().Contains(normalizedQuery, StringComparison.OrdinalIgnoreCase) ||
                appointment.Notes.Contains(normalizedQuery, StringComparison.OrdinalIgnoreCase) ||
                appointment.Time.Contains(normalizedQuery, StringComparison.OrdinalIgnoreCase));
        }

        return Ok(new ApiResponse<List<AppointmentDto>>
        {
            Success = true,
            Data = appointments.Select(appointment => appointment.ToDto()).ToList()
        });
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAppointmentRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userId = GetCurrentUserId();
        var appointment = await appointmentService.CreateAsync(request, userId);

        return CreatedAtAction(nameof(GetById), new { id = appointment.Id }, new ApiResponse<AppointmentDto>
        {
            Success = true,
            Data = appointment.ToDto()
        });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateAppointmentRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userId = GetCurrentUserId();
        var appointment = await appointmentService.UpdateAsync(id, request, userId);

        if (appointment == null)
            return NotFound(new ApiResponse { Success = false, Message = "Appointment not found" });

        return Ok(new ApiResponse<AppointmentDto>
        {
            Success = true,
            Message = "Appointment updated successfully",
            Data = appointment.ToDto()
        });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = GetCurrentUserId();
        var result = await appointmentService.DeleteAsync(id, userId);

        if (!result)
            return NotFound(new ApiResponse { Success = false, Message = "Appointment not found" });

        return Ok(new ApiResponse
        {
            Success = true,
            Message = "Appointment deleted successfully"
        });
    }

    [HttpGet("property/{propertyId}")]
    public async Task<IActionResult> GetByPropertyId(int propertyId)
    {
        var appointments = await appointmentService.GetByPropertyAsync(propertyId);

        return Ok(new ApiResponse<List<AppointmentDto>>
        {
            Success = true,
            Data = appointments.Select(appointment => appointment.ToDto()).ToList()
        });
    }

    [HttpGet("client/{clientId}")]
    public async Task<IActionResult> GetByClientId(int clientId)
    {
        var appointments = await appointmentService.GetByClientAsync(clientId);

        return Ok(new ApiResponse<List<AppointmentDto>>
        {
            Success = true,
            Data = appointments.Select(appointment => appointment.ToDto()).ToList()
        });
    }

    [HttpGet("by-date")]
    public async Task<IActionResult> GetByDateRange([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        var appointments = await appointmentService.GetByDateRangeAsync(startDate, endDate);

        return Ok(new ApiResponse<List<AppointmentDto>>
        {
            Success = true,
            Data = appointments.Select(appointment => appointment.ToDto()).ToList()
        });
    }

    [HttpGet("by-status/{status}")]
    public async Task<IActionResult> GetByStatus(AppointmentStatus status)
    {
        var userId = GetCurrentUserId();
        var appointments = await appointmentService.GetAllByUserAsync(userId);

        return Ok(new ApiResponse<List<AppointmentDto>>
        {
            Success = true,
            Data = appointments
                .Where(appointment => appointment.Status == status)
                .Select(appointment => appointment.ToDto())
                .ToList()
        });
    }

    [HttpGet("upcoming")]
    public async Task<IActionResult> GetUpcoming([FromQuery] int limit = 10)
    {
        var userId = GetCurrentUserId();
        var today = DateTime.UtcNow.Date;
        var appointments = await appointmentService.GetAllByUserAsync(userId);

        return Ok(new ApiResponse<List<AppointmentDto>>
        {
            Success = true,
            Data = appointments
                .Where(appointment => appointment.AppointmentDate.Date >= today && appointment.Status == AppointmentStatus.Scheduled)
                .OrderBy(appointment => appointment.AppointmentDate)
                .ThenBy(appointment => appointment.Time)
                .Take(limit)
                .Select(appointment => appointment.ToDto())
                .ToList()
        });
    }

    [HttpGet("today")]
    public async Task<IActionResult> GetToday()
    {
        var userId = GetCurrentUserId();
        var today = DateTime.UtcNow.Date;
        var appointments = await appointmentService.GetAllByUserAsync(userId);

        return Ok(new ApiResponse<List<AppointmentDto>>
        {
            Success = true,
            Data = appointments
                .Where(appointment => appointment.AppointmentDate.Date == today)
                .OrderBy(appointment => appointment.Time)
                .Select(appointment => appointment.ToDto())
                .ToList()
        });
    }

    [HttpPost("{id}/confirm")]
    public async Task<IActionResult> Confirm(int id)
    {
        return await UpdateStatus(id, new UpdateAppointmentStatusRequest { Status = AppointmentStatus.Scheduled });
    }

    [HttpPatch("{id}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateAppointmentStatusRequest request)
    {
        var userId = GetCurrentUserId();
        var appointment = await appointmentService.UpdateAsync(id, new UpdateAppointmentRequest { Status = request.Status }, userId);

        if (appointment == null)
            return NotFound(new ApiResponse { Success = false, Message = "Appointment not found" });

        return Ok(new ApiResponse<AppointmentDto>
        {
            Success = true,
            Message = "Appointment status updated successfully",
            Data = appointment.ToDto()
        });
    }
}


