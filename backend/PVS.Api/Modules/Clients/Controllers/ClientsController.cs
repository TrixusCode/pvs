using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PVS.Api.Common;
using PVS.Api.Modules.Clients.Dtos;
using PVS.Api.Modules.Clients.Mappers;
using PVS.Api.Modules.Clients.Services;

namespace PVS.Api.Modules.Clients.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ClientsController : ControllerBase
{
    private readonly IClientService _clientService;

    public ClientsController(IClientService clientService)
    {
        _clientService = clientService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var userId = GetCurrentUserId();
        if (userId == null)
            return Unauthorized(new ApiResponse { Success = false, Message = "User not authenticated" });

        var clients = await _clientService.GetAllAsync(userId.Value);

        return Ok(new ApiResponse<List<ClientDto>>
        {
            Success = true,
            Data = clients.Select(client => client.ToDto()).ToList()
        });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var userId = GetCurrentUserId();
        if (userId == null)
            return Unauthorized(new ApiResponse { Success = false, Message = "User not authenticated" });

        var client = await _clientService.GetByIdAsync(id, userId.Value);
        if (client == null)
            return NotFound(new ApiResponse { Success = false, Message = "Client not found" });

        return Ok(new ApiResponse<ClientDto>
        {
            Success = true,
            Data = client.ToDto()
        });
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateClientRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userId = GetCurrentUserId();
        if (userId == null)
            return Unauthorized(new ApiResponse { Success = false, Message = "User not authenticated" });

        var client = await _clientService.CreateAsync(request, userId.Value);

        return CreatedAtAction(nameof(GetById), new { id = client.Id }, new ApiResponse<ClientDto>
        {
            Success = true,
            Data = client.ToDto()
        });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateClientRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userId = GetCurrentUserId();
        if (userId == null)
            return Unauthorized(new ApiResponse { Success = false, Message = "User not authenticated" });

        var client = await _clientService.UpdateAsync(id, request, userId.Value);
        if (client == null)
            return NotFound(new ApiResponse { Success = false, Message = "Client not found or access denied" });

        return Ok(new ApiResponse<ClientDto>
        {
            Success = true,
            Data = client.ToDto()
        });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = GetCurrentUserId();
        if (userId == null)
            return Unauthorized(new ApiResponse { Success = false, Message = "User not authenticated" });

        var success = await _clientService.DeleteAsync(id, userId.Value);
        if (!success)
            return NotFound(new ApiResponse { Success = false, Message = "Client not found or access denied" });

        return Ok(new ApiResponse
        {
            Success = true,
            Message = "Client deleted successfully"
        });
    }

    private int? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(userIdClaim, out var userId) ? userId : null;
    }
}



