using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PVS.Api.Common;
using PVS.Api.Models;
using PVS.Api.Modules.Properties.Dtos;
using PVS.Api.Modules.Properties.Services;

namespace PVS.Api.Modules.Properties.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class PropertiesController(IPropertiesService propertiesService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var properties = await propertiesService.GetAllAsync(page, pageSize);
        var total = await propertiesService.GetTotalCountAsync();

        var skip = (page - 1) * pageSize;
        var items = properties.ToList();

        return Ok(new PaginatedResponse<Property>
        {
            Data = items,
            CurrentPage = page,
            PageSize = pageSize,
            TotalCount = total
        });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var property = await propertiesService.GetByIdAsync(id);
        if (property == null)
            return NotFound(new ApiResponse { Success = false, Message = "Property not found" });

        return Ok(new ApiResponse<Property>
        {
            Success = true,
            Data = property
        });
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePropertyRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userId = GetCurrentUserId();
        if (userId == null)
            return Unauthorized(new ApiResponse { Success = false, Message = "User not authenticated" });

        var property = await propertiesService.CreateAsync(request, userId.Value);

        return CreatedAtAction(nameof(GetById), new { id = property.Id }, new ApiResponse<Property>
        {
            Success = true,
            Data = property
        });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdatePropertyRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userId = GetCurrentUserId();
        if (userId == null)
            return Unauthorized(new ApiResponse { Success = false, Message = "User not authenticated" });

        var property = await propertiesService.UpdateAsync(id, request, userId.Value);
        if (property == null)
            return NotFound(new ApiResponse { Success = false, Message = "Property not found or access denied" });

        return Ok(new ApiResponse<Property>
        {
            Success = true,
            Data = property
        });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = GetCurrentUserId();
        if (userId == null)
            return Unauthorized(new ApiResponse { Success = false, Message = "User not authenticated" });

        var success = await propertiesService.DeleteAsync(id, userId.Value);
        if (!success)
            return NotFound(new ApiResponse { Success = false, Message = "Property not found or access denied" });

        return Ok(new ApiResponse
        {
            Success = true,
            Message = "Property deleted successfully"
        });
    }

    private int? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(userIdClaim, out var userId) ? userId : null;
    }
}
