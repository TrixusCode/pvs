using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PVS.Api.Common;
using PVS.Api.Modules.Appointments.Dtos;
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
}



