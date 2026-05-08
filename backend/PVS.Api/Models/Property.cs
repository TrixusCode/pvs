using PVS.Api.Modules.Properties.Enums;

namespace PVS.Api.Models;

public class Property
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public PropertyType PropertyType { get; set; }
    public string Description { get; set; } = string.Empty;
    public Address Address { get; set; } = new Address();
    public decimal Price { get; set; }
    public int Bedrooms { get; set; }
    public int Bathrooms { get; set; }
    public decimal SquareFeet { get; set; }
    public int UserId { get; set; }
    public string? ImagePath { get; set; }
    public int BranchId { get; set; }
    public Branch Branch { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}