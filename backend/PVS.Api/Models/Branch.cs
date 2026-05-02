using PVS.Api.Modules.Branches.Enums;

namespace PVS.Api.Models;

public class Branch
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Address Address { get; set; } = new Address();
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string ManagerName { get; set; } = string.Empty;
    public BranchStatus Status { get; set; } = BranchStatus.Active; 
    public int? ManagerUserId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}