using PVS.Api.Modules.Employees.Enums;

namespace PVS.Api.Models;

public class Employee
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public DateTime Birthdate { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public int AddressId { get; set; }
    public Address Address { get; set; } = new Address();
    public int BranchId { get; set; }
    public Branch Branch { get; set; } = null!;
    public EmployeeRole Role { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsClient { get; set; }
    public string? ImagePath { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ModifiedAt { get; set; }
}

