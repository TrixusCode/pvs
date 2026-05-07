namespace PVS.Api.Modules.Branches.Dtos;

using PVS.Api.Models;
using PVS.Api.Modules.Branches.Enums;

public class CreateBranchDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Address Address { get; set; } = new();
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string ManagerName { get; set; } = string.Empty;
    public BranchStatus? Status { get; set; }
    public int? ManagerUserId { get; set; }
}
