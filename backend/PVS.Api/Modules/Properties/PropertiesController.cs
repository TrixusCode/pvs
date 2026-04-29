using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PVS.Api.Common;
using PVS.Api.Models;

namespace PVS.Api.Modules.Properties;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PropertiesController : ControllerBase
{
    // Mock data for demo
    private static readonly List<Property> Properties = new()
    {
        new Property
        {
            Id = 1,
            Title = "Beautiful House in Downtown",
            Description = "3 bedroom, 2 bathroom house with modern amenities",
            Address = "123 Main St",
            City = "New York",
            State = "NY",
            ZipCode = "10001",
            Price = 450000,
            Bedrooms = 3,
            Bathrooms = 2,
            SquareFeet = 2500,
            UserId = 1,
            CreatedAt = DateTime.UtcNow.AddDays(-30)
        },
        new Property
        {
            Id = 2,
            Title = "Cozy Apartment",
            Description = "1 bedroom apartment near the park",
            Address = "456 Oak Ave",
            City = "New York",
            State = "NY",
            ZipCode = "10002",
            Price = 250000,
            Bedrooms = 1,
            Bathrooms = 1,
            SquareFeet = 800,
            UserId = 1,
            CreatedAt = DateTime.UtcNow.AddDays(-15)
        }
    };

    [HttpGet]
    public IActionResult GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var skip = (page - 1) * pageSize;
        var total = Properties.Count;
        var items = Properties.Skip(skip).Take(pageSize).ToList();

        return Ok(new PaginatedResponse<Property>
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
        var property = Properties.FirstOrDefault(p => p.Id == id);
        if (property == null)
            return NotFound(new ApiResponse { Success = false, Message = "Property not found" });

        return Ok(new ApiResponse<Property>
        {
            Success = true,
            Data = property
        });
    }

    [HttpPost]
    public IActionResult Create([FromBody] CreatePropertyRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var property = new Property
        {
            Id = Properties.Max(p => p.Id) + 1,
            Title = request.Title,
            Description = request.Description,
            Address = request.Address,
            City = request.City,
            State = request.State,
            ZipCode = request.ZipCode,
            Price = request.Price,
            Bedrooms = request.Bedrooms,
            Bathrooms = request.Bathrooms,
            SquareFeet = request.SquareFeet,
            UserId = 1, // TODO: Get from authenticated user
            CreatedAt = DateTime.UtcNow
        };

        Properties.Add(property);

        return CreatedAtAction(nameof(GetById), new { id = property.Id }, property);
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, [FromBody] UpdatePropertyRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var property = Properties.FirstOrDefault(p => p.Id == id);
        if (property == null)
            return NotFound(new ApiResponse { Success = false, Message = "Property not found" });

        property.Title = request.Title ?? property.Title;
        property.Description = request.Description ?? property.Description;
        property.Address = request.Address ?? property.Address;
        property.City = request.City ?? property.City;
        property.State = request.State ?? property.State;
        property.ZipCode = request.ZipCode ?? property.ZipCode;
        property.Price = request.Price ?? property.Price;
        property.Bedrooms = request.Bedrooms ?? property.Bedrooms;
        property.Bathrooms = request.Bathrooms ?? property.Bathrooms;
        property.SquareFeet = request.SquareFeet ?? property.SquareFeet;
        property.UpdatedAt = DateTime.UtcNow;

        return Ok(new ApiResponse<Property>
        {
            Success = true,
            Message = "Property updated successfully",
            Data = property
        });
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var property = Properties.FirstOrDefault(p => p.Id == id);
        if (property == null)
            return NotFound(new ApiResponse { Success = false, Message = "Property not found" });

        Properties.Remove(property);

        return Ok(new ApiResponse
        {
            Success = true,
            Message = "Property deleted successfully"
        });
    }

    [HttpGet("search")]
    public IActionResult Search([FromQuery] string q)
    {
        var results = Properties
            .Where(p => p.Title.Contains(q, StringComparison.OrdinalIgnoreCase) ||
                       p.Address.Contains(q, StringComparison.OrdinalIgnoreCase))
            .ToList();

        return Ok(new ApiResponse<List<Property>>
        {
            Success = true,
            Data = results
        });
    }
}

public class CreatePropertyRequest
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string ZipCode { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Bedrooms { get; set; }
    public int Bathrooms { get; set; }
    public decimal SquareFeet { get; set; }
}

public class UpdatePropertyRequest
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? ZipCode { get; set; }
    public decimal? Price { get; set; }
    public int? Bedrooms { get; set; }
    public int? Bathrooms { get; set; }
    public decimal? SquareFeet { get; set; }
}
