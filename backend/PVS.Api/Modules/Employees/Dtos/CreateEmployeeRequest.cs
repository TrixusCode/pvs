using System.ComponentModel.DataAnnotations;
using PVS.Api.Models;
using PVS.Api.Modules.Employees.Enums;

namespace PVS.Api.Modules.Employees.Dtos;

public class CreateEmployeeRequest
{
    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [Phone]
    public string PhoneNumber { get; set; } = string.Empty;

    [Required]
    public DateTime Birthdate { get; set; }

    [Required]
    public int UserId { get; set; }

    [Required]
    public int BranchId { get; set; }

    [Required]
    public EmployeeRole Role { get; set; }

    public bool IsActive { get; set; } = true;
    public bool IsClient { get; set; }

    [Required]
    public Address Address { get; set; } = new();
}
