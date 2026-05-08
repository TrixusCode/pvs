using System.ComponentModel.DataAnnotations;
using PVS.Api.Models;
using PVS.Api.Modules.Employees.Enums;

namespace PVS.Api.Modules.Employees.Dtos;

public class UpdateEmployeeRequest
{
    [MaxLength(100)]
    public string? FirstName { get; set; }

    [MaxLength(100)]
    public string? LastName { get; set; }

    [Phone]
    public string? PhoneNumber { get; set; }

    public DateTime? Birthdate { get; set; }
    public int? BranchId { get; set; }
    public EmployeeRole? Role { get; set; }
    public bool? IsActive { get; set; }
    public bool? IsClient { get; set; }
    public Address? Address { get; set; }
}
