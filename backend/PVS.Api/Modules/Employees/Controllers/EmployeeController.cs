using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PVS.Api.Common;
using PVS.Api.Modules.Employees.Dtos;
using PVS.Api.Modules.Employees.Mappers;
using PVS.Api.Modules.Employees.Services;

namespace PVS.Api.Modules.Employees.Controllers;

[ApiController]
[Route("api/employees")]
[Authorize]
public class EmployeeController : ControllerBase
{
    private readonly IEmployeeService _employeeService;

    public EmployeeController(IEmployeeService employeeService)
    {
        _employeeService = employeeService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null,
        [FromQuery] int? branchId = null,
        [FromQuery] string? role = null)
    {
        var employees = await _employeeService.GetAllAsync(page, pageSize, search, branchId, role);
        var total = await _employeeService.GetTotalCountAsync(search, branchId, role);

        return Ok(new PaginatedResponse<EmployeeDto>
        {
            Data = employees.Select(e => e.ToDto()).ToList(),
            CurrentPage = page,
            PageSize = pageSize,
            TotalCount = total
        });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var employee = await _employeeService.GetByIdAsync(id);
        if (employee == null)
            return NotFound(new ApiResponse { Success = false, Message = "Employee not found" });

        return Ok(new ApiResponse<EmployeeDto>
        {
            Success = true,
            Data = employee.ToDto()
        });
    }

    [HttpGet("branch/{branchId}")]
    public async Task<IActionResult> GetByBranch(int branchId)
    {
        var employees = await _employeeService.GetByBranchAsync(branchId);
        return Ok(new ApiResponse<List<EmployeeDto>>
        {
            Success = true,
            Data = employees.Select(e => e.ToDto()).ToList()
        });
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Create([FromBody] CreateEmployeeRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var employee = await _employeeService.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = employee.Id }, new ApiResponse<EmployeeDto>
            {
                Success = true,
                Data = employee.ToDto()
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new ApiResponse { Success = false, Message = ex.Message });
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateEmployeeRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var employee = await _employeeService.UpdateAsync(id, request);
            if (employee == null)
                return NotFound(new ApiResponse { Success = false, Message = "Employee not found" });

            return Ok(new ApiResponse<EmployeeDto>
            {
                Success = true,
                Data = employee.ToDto()
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new ApiResponse { Success = false, Message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _employeeService.DeleteAsync(id);
        if (!result)
            return NotFound(new ApiResponse { Success = false, Message = "Employee not found" });

        return Ok(new ApiResponse
        {
            Success = true,
            Message = "Employee deleted successfully"
        });
    }

    [HttpPost("{id}/upload-image")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> UploadImage(int id, IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest(new ApiResponse { Success = false, Message = "Image file is required" });

        var employee = await _employeeService.UploadImageAsync(id, file);
        if (employee == null)
            return NotFound(new ApiResponse { Success = false, Message = "Employee not found" });

        return Ok(new ApiResponse<EmployeeDto>
        {
            Success = true,
            Data = employee.ToDto()
        });
    }
}
