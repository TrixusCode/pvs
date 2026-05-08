using PVS.Api.Models;
using PVS.Api.Modules.Employees.Enums;

namespace PVS.Api.Modules.Employees.Dtos;

public class EmployeeDto
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public DateTime Birthdate { get; set; }
    public int BranchId { get; set; }
    public string BranchName { get; set; } = string.Empty;
    public string BranchPhone { get; set; } = string.Empty;
    public string BranchEmail { get; set; } = string.Empty;
    public EmployeeRole Role { get; set; }
    public bool IsActive { get; set; }
    public bool IsClient { get; set; }
    public string? ImagePath { get; set; }
    public int UserId { get; set; }
    public Address Address { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime? ModifiedAt { get; set; }
}
