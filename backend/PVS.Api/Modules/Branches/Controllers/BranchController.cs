using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PVS.Api.Common;
using PVS.Api.Data;
using PVS.Api.Models;
using PVS.Api.Modules.Branches.Dtos;
using PVS.Api.Modules.Branches.Mappers;
using PVS.Api.Modules.Auth.Dtos;
using PVS.Api.Modules.Auth.Mappers;
using PVS.Api.Modules.Branches.Enums;
using PVS.Api.Modules.Employees.Mappers;
using PVS.Api.Modules.Employees.Dtos;

namespace PVS.Api.Modules.Branches.Controllers;

[ApiController]
[Route("api/branches")]
[Authorize]
public class BranchController(AppDbContext context) : ControllerBase
{
    [HttpGet]
    public IActionResult GetAll()
    {
        var branches = context.Branches.Include(b => b.Address).Include(b => b.Employees).ToList();
        return Ok(new ApiResponse<List<BranchDto>>
        {
            Success = true,
            Data = branches.Select(branch => branch.ToDto()).ToList()
        });
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var branch = context.Branches.Include(b => b.Address).Include(b => b.Employees).FirstOrDefault(b => b.Id == id);
        if (branch == null)
            return NotFound(new ApiResponse { Success = false, Message = "Branch not found" });

        return Ok(new ApiResponse<BranchDto>
        {
            Success = true,
            Data = branch.ToDto()
        });
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Create([FromBody] CreateBranchDto request)
    {
        if (!ModelState.IsValid)
            return BadRequest(new ApiResponse { Success = false, Message = "Invalid input" });

        var branch = new Branch
        {
            Name = request.Name,
            Description = request.Description,
            Address = request.Address ?? new Address(),
            Phone = request.Phone,
            Email = request.Email,
            ManagerName = request.ManagerName,
            Status = request.Status ?? BranchStatus.Active,
            ManagerUserId = request.ManagerUserId,
            CreatedAt = DateTime.UtcNow
        };

        context.Branches.Add(branch);
        await context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = branch.Id },
            new ApiResponse<BranchDto>
            {
                Success = true,
                Message = "Branch created successfully",
                Data = branch.ToDto()
            });
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateBranchDto request)
    {
        if (!ModelState.IsValid)
            return BadRequest(new ApiResponse { Success = false, Message = "Invalid input" });

        var branch = await context.Branches.Include(b => b.Address).FirstOrDefaultAsync(b => b.Id == id);
        if (branch == null)
            return NotFound(new ApiResponse { Success = false, Message = "Branch not found" });

        branch.Name = request.Name ?? branch.Name;
        branch.Description = request.Description ?? branch.Description;
        branch.Phone = request.Phone ?? branch.Phone;
        branch.Email = request.Email ?? branch.Email;
        branch.ManagerName = request.ManagerName ?? branch.ManagerName;
        branch.ManagerUserId = request.ManagerUserId ?? branch.ManagerUserId;

        if (request.Address != null)
        {
            branch.Address ??= new Address();
            branch.Address.City = request.Address.City ?? branch.Address.City;
            branch.Address.State = request.Address.State ?? branch.Address.State;
            branch.Address.ZipCode = request.Address.ZipCode ?? branch.Address.ZipCode;
        }

        await context.SaveChangesAsync();

        return Ok(new ApiResponse<BranchDto>
        {
            Success = true,
            Message = "Branch updated successfully",
            Data = branch.ToDto()
        });
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var branch = await context.Branches.FindAsync(id);
        if (branch == null)
            return NotFound(new ApiResponse { Success = false, Message = "Branch not found" });

        context.Branches.Remove(branch);
        await context.SaveChangesAsync();

        return Ok(new ApiResponse
        {
            Success = true,
            Message = "Branch deleted successfully"
        });
    }

    [HttpGet("{id}/staff")]
    public IActionResult GetBranchStaff(int id)
    {
        var branch = context.Branches.Find(id);
        if (branch == null)
            return NotFound(new ApiResponse { Success = false, Message = "Branch not found" });

        var staff = context.Employees.Where(e => e.BranchId == id).ToList();
        return Ok(new ApiResponse<List<EmployeeDto>>
        {
            Success = true,
            Data = staff.Select(e => e.ToDto()).ToList()
        });
    }
}
