using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PVS.Api.Common;
using PVS.Api.Models;
using PVS.Api.Modules.Appointments.Dtos;
using PVS.Api.Modules.Appointments.Services;

namespace PVS.Api.Modules.Appointments.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AppointmentsController : ControllerBase
{
    private readonly IAppointmentService _appointmentService;

    public AppointmentsController(IAppointmentService appointmentService)
    {
        _appointmentService = appointmentService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var userId = GetCurrentUserId();
        if (userId == null)
            return Unauthorized(new ApiResponse { Success = false, Message = "User not authenticated" });

        var appointments = await _appointmentService.GetAllByUserAsync(userId.Value);
        var skip = (page - 1) * pageSize;
        var items = appointments.Skip(skip).Take(pageSize).ToList();

        return Ok(new PaginatedResponse<Appointment>
        {
            Data = items,
            CurrentPage = page,
            PageSize = pageSize,
            TotalCount = appointments.Count()
        });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var appointment = await _appointmentService.GetByIdAsync(id);
        if (appointment == null)
            return NotFound(new ApiResponse { Success = false, Message = "Appointment not found" });

        return Ok(new ApiResponse<Appointment>
        {
            Success = true,
            Data = appointment
        });
    }

    [HttpGet("property/{propertyId}")]
    public async Task<IActionResult> GetByProperty(int propertyId)
    {
        var appointments = await _appointmentService.GetByPropertyAsync(propertyId);
        return Ok(new ApiResponse<List<Appointment>>
        {
            Success = true,
            Data = appointments.ToList()
        });
    }

    [HttpGet("client/{clientId}")]
    public async Task<IActionResult> GetByClient(int clientId)
    {
        var appointments = await _appointmentService.GetByClientAsync(clientId);
        return Ok(new ApiResponse<List<Appointment>>
        {
            Success = true,
            Data = appointments.ToList()
        });
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAppointmentRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userId = GetCurrentUserId();
        if (userId == null)
            return Unauthorized(new ApiResponse { Success = false, Message = "User not authenticated" });

        var appointment = await _appointmentService.CreateAsync(request, userId.Value);

        return CreatedAtAction(nameof(GetById), new { id = appointment.Id }, new ApiResponse<Appointment>
        {
            Success = true,
            Data = appointment
        });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateAppointmentRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userId = GetCurrentUserId();
        if (userId == null)
            return Unauthorized(new ApiResponse { Success = false, Message = "User not authenticated" });

        var appointment = await _appointmentService.UpdateAsync(id, request, userId.Value);
        if (appointment == null)
            return NotFound(new ApiResponse { Success = false, Message = "Appointment not found or access denied" });

        return Ok(new ApiResponse<Appointment>
        {
            Success = true,
            Data = appointment
        });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = GetCurrentUserId();
        if (userId == null)
            return Unauthorized(new ApiResponse { Success = false, Message = "User not authenticated" });

        var success = await _appointmentService.DeleteAsync(id, userId.Value);
        if (!success)
            return NotFound(new ApiResponse { Success = false, Message = "Appointment not found or access denied" });

        return Ok(new ApiResponse
        {
            Success = true,
            Message = "Appointment deleted successfully"
        });
    }

    private int? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(userIdClaim, out var userId) ? userId : null;
    }
}
