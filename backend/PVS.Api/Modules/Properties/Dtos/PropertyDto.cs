namespace PVS.Api.Modules.Properties.Dtos;

using PVS.Api.Models;
using PVS.Api.Modules.Properties.Enums;

public class PropertyDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public PropertyType PropertyType { get; set; }
    public string Description { get; set; } = string.Empty;
    public Address Address { get; set; } = new();
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string ZipCode { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Bedrooms { get; set; }
    public int Bathrooms { get; set; }
    public decimal SquareFeet { get; set; }
    public string? ImagePath { get; set; }
    public int BranchId { get; set; }
    public int UserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
