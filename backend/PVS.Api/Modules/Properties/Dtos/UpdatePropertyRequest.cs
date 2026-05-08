using System.ComponentModel.DataAnnotations;
using PVS.Api.Modules.Properties.Enums;

namespace PVS.Api.Modules.Properties.Dtos;

public class UpdatePropertyRequest
{
    [MaxLength(255)]
    public string? Title { get; set; }

    [MaxLength(1000)]
    public string? Description { get; set; }
    public int? BranchId { get; set; }
    public PropertyType? PropertyType { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? ZipCode { get; set; }

    [Range(0, double.MaxValue)]
    public decimal? Price { get; set; }

    [Range(0, 100)]
    public int? Bedrooms { get; set; }

    [Range(0, 100)]
    public int? Bathrooms { get; set; }

    [Range(0, double.MaxValue)]
    public decimal? SquareFeet { get; set; }
}