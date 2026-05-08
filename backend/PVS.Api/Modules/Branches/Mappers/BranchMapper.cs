using PVS.Api.Models;
using PVS.Api.Modules.Branches.Dtos;
using PVS.Api.Modules.Branches.Enums;

namespace PVS.Api.Modules.Branches.Mappers;

public static class BranchMapper
{
    public static BranchDto ToDto(this Branch branch)
    {
        return new BranchDto
        {
            Id = branch.Id,
            Name = branch.Name,
            Description = branch.Description,
            Address = branch.Address,
            Phone = branch.Phone,
            Email = branch.Email,
            ManagerName = branch.ManagerName,
            Status = branch.Status,
            EmployeeCount = branch.Employees?.Count ?? 0,
            ManagerUserId = branch.ManagerUserId,
            CreatedAt = branch.CreatedAt,
            UpdatedAt = branch.UpdatedAt
        };
    }

    public static Branch ToEntity(this CreateBranchDto request)
    {
        return new Branch
        {
            Name = request.Name,
            Description = request.Description,
            Address = request.Address,
            Phone = request.Phone,
            Email = request.Email,
            ManagerName = request.ManagerName,
            Status = request.Status ?? BranchStatus.Active,
            ManagerUserId = request.ManagerUserId,
            CreatedAt = DateTime.UtcNow
        };
    }

    public static void ApplyTo(this UpdateBranchDto request, Branch branch)
    {
        if (request.Name != null) branch.Name = request.Name;
        if (request.Description != null) branch.Description = request.Description;
        if (request.Address != null) branch.Address = request.Address;
        if (request.Phone != null) branch.Phone = request.Phone;
        if (request.Email != null) branch.Email = request.Email;
        if (request.ManagerName != null) branch.ManagerName = request.ManagerName;
        if (request.Status.HasValue) branch.Status = request.Status.Value;
        if (request.ManagerUserId.HasValue) branch.ManagerUserId = request.ManagerUserId;

        branch.UpdatedAt = DateTime.UtcNow;
    }
}
