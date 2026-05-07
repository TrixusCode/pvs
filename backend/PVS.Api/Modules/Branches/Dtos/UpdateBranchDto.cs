namespace PVS.Api.Modules.Branches.Dtos;

using PVS.Api.Models;
using PVS.Api.Modules.Branches.Enums;

public class UpdateBranchDto
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public Address? Address { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? ManagerName { get; set; }
    public BranchStatus? Status { get; set; }
    public int? ManagerUserId { get; set; }
}
