using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PVS.Api.Common;
using PVS.Api.Models;

namespace PVS.Api.Modules.Clients;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ClientsController : ControllerBase
{
    // Mock data for demo
    private static readonly List<Client> Clients = new()
    {
        new Client
        {
            Id = 1,
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com",
            Phone = "(555) 123-4567",
            Address = "123 Oak Street",
            City = "New York",
            State = "NY",
            ZipCode = "10001",
            ClientType = "Buyer",
            Status = "Active",
            UserId = 1,
            CreatedAt = DateTime.UtcNow.AddDays(-30)
        },
        new Client
        {
            Id = 2,
            FirstName = "Jane",
            LastName = "Smith",
            Email = "jane.smith@example.com",
            Phone = "(555) 234-5678",
            Address = "456 Maple Avenue",
            City = "Los Angeles",
            State = "CA",
            ZipCode = "90001",
            ClientType = "Seller",
            Status = "Active",
            UserId = 1,
            CreatedAt = DateTime.UtcNow.AddDays(-15)
        }
    };

    [HttpGet]
    public IActionResult GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var skip = (page - 1) * pageSize;
        var total = Clients.Count;
        var items = Clients.Skip(skip).Take(pageSize).ToList();

        return Ok(new PaginatedResponse<Client>
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
        var client = Clients.FirstOrDefault(c => c.Id == id);
        if (client == null)
            return NotFound(new ApiResponse { Success = false, Message = "Client not found" });

        return Ok(new ApiResponse<Client>
        {
            Success = true,
            Data = client
        });
    }

    [HttpPost]
    public IActionResult Create([FromBody] CreateClientRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var client = new Client
        {
            Id = Clients.Max(c => c.Id) + 1,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Phone = request.Phone,
            Address = request.Address,
            City = request.City,
            State = request.State,
            ZipCode = request.ZipCode,
            ClientType = request.ClientType ?? "Buyer",
            Status = request.Status ?? "Active",
            UserId = 1, // TODO: Get from authenticated user
            CreatedAt = DateTime.UtcNow
        };

        Clients.Add(client);
        return CreatedAtAction(nameof(GetById), new { id = client.Id }, client);
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, [FromBody] UpdateClientRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var client = Clients.FirstOrDefault(c => c.Id == id);
        if (client == null)
            return NotFound(new ApiResponse { Success = false, Message = "Client not found" });

        if (!string.IsNullOrEmpty(request.FirstName)) client.FirstName = request.FirstName;
        if (!string.IsNullOrEmpty(request.LastName)) client.LastName = request.LastName;
        if (!string.IsNullOrEmpty(request.Email)) client.Email = request.Email;
        if (!string.IsNullOrEmpty(request.Phone)) client.Phone = request.Phone;
        if (!string.IsNullOrEmpty(request.Address)) client.Address = request.Address;
        if (!string.IsNullOrEmpty(request.City)) client.City = request.City;
        if (!string.IsNullOrEmpty(request.State)) client.State = request.State;
        if (!string.IsNullOrEmpty(request.ZipCode)) client.ZipCode = request.ZipCode;
        if (!string.IsNullOrEmpty(request.ClientType)) client.ClientType = request.ClientType;
        if (!string.IsNullOrEmpty(request.Status)) client.Status = request.Status;
        client.UpdatedAt = DateTime.UtcNow;

        return Ok(new ApiResponse<Client>
        {
            Success = true,
            Message = "Client updated successfully",
            Data = client
        });
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var client = Clients.FirstOrDefault(c => c.Id == id);
        if (client == null)
            return NotFound(new ApiResponse { Success = false, Message = "Client not found" });

        Clients.Remove(client);

        return Ok(new ApiResponse
        {
            Success = true,
            Message = "Client deleted successfully"
        });
    }

    [HttpGet("search")]
    public IActionResult Search([FromQuery] string q)
    {
        var results = Clients
            .Where(c => c.FirstName.Contains(q, StringComparison.OrdinalIgnoreCase) ||
                       c.LastName.Contains(q, StringComparison.OrdinalIgnoreCase) ||
                       c.Email.Contains(q, StringComparison.OrdinalIgnoreCase))
            .ToList();

        return Ok(new ApiResponse<List<Client>>
        {
            Success = true,
            Data = results
        });
    }

    [HttpGet("by-type/{type}")]
    public IActionResult GetByType(string type)
    {
        var results = Clients
            .Where(c => c.ClientType.Equals(type, StringComparison.OrdinalIgnoreCase))
            .ToList();

        return Ok(new ApiResponse<List<Client>>
        {
            Success = true,
            Data = results
        });
    }
}

public class CreateClientRequest
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string ZipCode { get; set; } = string.Empty;
    public string? ClientType { get; set; }
    public string? Status { get; set; }
}

public class UpdateClientRequest
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? ZipCode { get; set; }
    public string? ClientType { get; set; }
    public string? Status { get; set; }
}
