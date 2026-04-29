using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PVS.Api.Common;
using PVS.Api.Models;

namespace PVS.Api.Modules.Appointments;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AppointmentsController : ControllerBase
{
    // Mock data for demo
    private static readonly List<Appointment> Appointments = new()
    {
        new Appointment
        {
            Id = 1,
            PropertyId = 1,
            ClientId = 1,
            AppointmentDate = DateTime.UtcNow.AddDays(3),
            Time = "10:00 AM",
            Type = "Showing",
            Status = "Scheduled",
            Notes = "Initial property showing",
            DurationMinutes = 60,
            UserId = 1,
            CreatedAt = DateTime.UtcNow.AddDays(-5)
        },
        new Appointment
        {
            Id = 2,
            PropertyId = 2,
            ClientId = 2,
            AppointmentDate = DateTime.UtcNow.AddDays(5),
            Time = "2:00 PM",
            Type = "Inspection",
            Status = "Scheduled",
            Notes = "Home inspection appointment",
            DurationMinutes = 120,
            UserId = 1,
            CreatedAt = DateTime.UtcNow.AddDays(-2)
        }
    };

    [HttpGet]
    public IActionResult GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var skip = (page - 1) * pageSize;
        var total = Appointments.Count;
        var items = Appointments.Skip(skip).Take(pageSize).ToList();

        return Ok(new PaginatedResponse<Appointment>
        {
            Data = items,
            CurrentPage = page,
            PageSize = pageSize,
            TotalCount = total
        });
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var appointment = Appointments.FirstOrDefault(a => a.Id == id);
        if (appointment == null)
            return NotFound(new ApiResponse { Success = false, Message = "Appointment not found" });

        return Ok(new ApiResponse<Appointment>
        {
            Success = true,
            Data = appointment
        });
    }

    [HttpPost]
    public IActionResult Create([FromBody] CreateAppointmentRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var appointment = new Appointment
        {
            Id = Appointments.Max(a => a.Id) + 1,
            PropertyId = request.PropertyId,
            ClientId = request.ClientId,
            AppointmentDate = request.AppointmentDate,
            Time = request.Time,
            Type = request.Type ?? "Showing",
            Status = request.Status ?? "Scheduled",
            Notes = request.Notes ?? string.Empty,
            DurationMinutes = request.DurationMinutes,
            UserId = 1, // TODO: Get from authenticated user
            CreatedAt = DateTime.UtcNow
        };

        Appointments.Add(appointment);
        return CreatedAtAction(nameof(GetById), new { id = appointment.Id }, appointment);
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, [FromBody] UpdateAppointmentRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var appointment = Appointments.FirstOrDefault(a => a.Id == id);
        if (appointment == null)
            return NotFound(new ApiResponse { Success = false, Message = "Appointment not found" });

        if (request.AppointmentDate != default) appointment.AppointmentDate = request.AppointmentDate;
        if (!string.IsNullOrEmpty(request.Time)) appointment.Time = request.Time;
        if (!string.IsNullOrEmpty(request.Type)) appointment.Type = request.Type;
        if (!string.IsNullOrEmpty(request.Status)) appointment.Status = request.Status;
        if (!string.IsNullOrEmpty(request.Notes)) appointment.Notes = request.Notes;
        if (request.DurationMinutes.HasValue) appointment.DurationMinutes = request.DurationMinutes;
        if (!string.IsNullOrEmpty(request.FeedbackFromClient)) appointment.FeedbackFromClient = request.FeedbackFromClient;
        appointment.UpdatedAt = DateTime.UtcNow;

        return Ok(new ApiResponse<Appointment>
        {
            Success = true,
            Message = "Appointment updated successfully",
            Data = appointment
        });
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var appointment = Appointments.FirstOrDefault(a => a.Id == id);
        if (appointment == null)
            return NotFound(new ApiResponse { Success = false, Message = "Appointment not found" });

        Appointments.Remove(appointment);

        return Ok(new ApiResponse
        {
            Success = true,
            Message = "Appointment deleted successfully"
        });
    }

    [HttpGet("property/{propertyId}")]
    public IActionResult GetByPropertyId(int propertyId)
    {
        var appointments = Appointments
            .Where(a => a.PropertyId == propertyId)
            .ToList();

        return Ok(new ApiResponse<List<Appointment>>
        {
            Success = true,
            Data = appointments
        });
    }

    [HttpGet("client/{clientId}")]
    public IActionResult GetByClientId(int clientId)
    {
        var appointments = Appointments
            .Where(a => a.ClientId == clientId)
            .ToList();

        return Ok(new ApiResponse<List<Appointment>>
        {
            Success = true,
            Data = appointments
        });
    }

    [HttpGet("by-status/{status}")]
    public IActionResult GetByStatus(string status)
    {
        var appointments = Appointments
            .Where(a => a.Status.Equals(status, StringComparison.OrdinalIgnoreCase))
            .ToList();

        return Ok(new ApiResponse<List<Appointment>>
        {
            Success = true,
            Data = appointments
        });
    }

    [HttpGet("by-date")]
    public IActionResult GetByDateRange([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        var appointments = Appointments
            .Where(a => a.AppointmentDate >= startDate && a.AppointmentDate <= endDate)
            .OrderBy(a => a.AppointmentDate)
            .ToList();

        return Ok(new ApiResponse<List<Appointment>>
        {
            Success = true,
            Data = appointments
        });
    }
}

public class CreateAppointmentRequest
{
    public int PropertyId { get; set; }
    public int ClientId { get; set; }
    public DateTime AppointmentDate { get; set; }
    public string Time { get; set; } = string.Empty;
    public string? Type { get; set; }
    public string? Status { get; set; }
    public string? Notes { get; set; }
    public decimal? DurationMinutes { get; set; }
}

public class UpdateAppointmentRequest
{
    public DateTime AppointmentDate { get; set; }
    public string? Time { get; set; }
    public string? Type { get; set; }
    public string? Status { get; set; }
    public string? Notes { get; set; }
    public decimal? DurationMinutes { get; set; }
    public string? FeedbackFromClient { get; set; }
}
