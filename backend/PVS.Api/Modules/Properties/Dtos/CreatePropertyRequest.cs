using System.ComponentModel.DataAnnotations;
using PVS.Api.Modules.Properties.Enums;

namespace PVS.Api.Modules.Properties.Dtos;

public class CreatePropertyRequest
{
    [Required]
    [MaxLength(255)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [MaxLength(1000)]
    public string Description { get; set; } = string.Empty;

    [Required]
    public int BranchId { get; set; }

    [Required]
    public PropertyType PropertyType { get; set; }

    [Required]
    public string City { get; set; } = string.Empty;

    [Required]
    public string State { get; set; } = string.Empty;

    [Required]
    public string ZipCode { get; set; } = string.Empty;

    [Required]
    [Range(0, double.MaxValue)]
    public decimal Price { get; set; }

    [Range(0, 100)]
    public int Bedrooms { get; set; }

    [Range(0, 100)]
    public int Bathrooms { get; set; }

    [Range(0, double.MaxValue)]
    public decimal SquareFeet { get; set; }
}