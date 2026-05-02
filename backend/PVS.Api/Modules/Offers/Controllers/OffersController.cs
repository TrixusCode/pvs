using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PVS.Api.Common;
using PVS.Api.Models;
using PVS.Api.Modules.Offers.Dtos;
using PVS.Api.Modules.Offers.Services;
using PVS.Api.Modules.Offers.Enums;
using System.Security.Claims;

namespace PVS.Api.Modules.Offers.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OffersController : ControllerBase
{
    private readonly IOfferService _offerService;

    public OffersController(IOfferService offerService)
    {
        _offerService = offerService;
    }

    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(userIdClaim, out var userId) ? userId : 0;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var userId = GetCurrentUserId();
        var offers = await _offerService.GetAllByUserAsync(userId);

        var skip = (page - 1) * pageSize;
        var total = offers.Count();
        var items = offers.Skip(skip).Take(pageSize).ToList();

        return Ok(new PaginatedResponse<Offer>
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
        var offer = await _offerService.GetByIdAsync(id);
        if (offer == null)
            return NotFound(new ApiResponse { Success = false, Message = "Offer not found" });

        return Ok(new ApiResponse<Offer>
        {
            Success = true,
            Data = offer
        });
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateOfferRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userId = GetCurrentUserId();
        var offer = await _offerService.CreateAsync(request, userId);

        return CreatedAtAction(nameof(GetById), new { id = offer.Id }, offer);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateOfferRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userId = GetCurrentUserId();
        var offer = await _offerService.UpdateAsync(id, request, userId);

        if (offer == null)
            return NotFound(new ApiResponse { Success = false, Message = "Offer not found" });

        return Ok(new ApiResponse<Offer>
        {
            Success = true,
            Message = "Offer updated successfully",
            Data = offer
        });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = GetCurrentUserId();
        var result = await _offerService.DeleteAsync(id, userId);

        if (!result)
            return NotFound(new ApiResponse { Success = false, Message = "Offer not found" });

        return Ok(new ApiResponse
        {
            Success = true,
            Message = "Offer deleted successfully"
        });
    }

    [HttpGet("property/{propertyId}")]
    public async Task<IActionResult> GetByPropertyId(int propertyId)
    {
        var offers = await _offerService.GetByPropertyAsync(propertyId);

        return Ok(new ApiResponse<List<Offer>>
        {
            Success = true,
            Data = offers.ToList()
        });
    }

    [HttpGet("client/{clientId}")]
    public async Task<IActionResult> GetByClientId(int clientId)
    {
        var offers = await _offerService.GetByClientAsync(clientId);

        return Ok(new ApiResponse<List<Offer>>
        {
            Success = true,
            Data = offers.ToList()
        });
    }

    [HttpPost("{id}/accept")]
    public async Task<IActionResult> AcceptOffer(int id)
    {
        var userId = GetCurrentUserId();
        var offer = await _offerService.UpdateAsync(id, new UpdateOfferRequest { Status = OfferStatus.Accepted }, userId);

        if (offer == null)
            return NotFound(new ApiResponse { Success = false, Message = "Offer not found" });

        return Ok(new ApiResponse<Offer>
        {
            Success = true,
            Message = "Offer accepted successfully",
            Data = offer
        });
    }

    [HttpPost("{id}/reject")]
    public async Task<IActionResult> RejectOffer(int id)
    {
        var userId = GetCurrentUserId();
        var offer = await _offerService.UpdateAsync(id, new UpdateOfferRequest { Status = OfferStatus.Rejected }, userId);

        if (offer == null)
            return NotFound(new ApiResponse { Success = false, Message = "Offer not found" });

        return Ok(new ApiResponse<Offer>
        {
            Success = true,
            Message = "Offer rejected successfully",
            Data = offer
        });
    }

    [HttpPost("{id}/withdraw")]
    public async Task<IActionResult> WithdrawOffer(int id)
    {
        var userId = GetCurrentUserId();
        var offer = await _offerService.UpdateAsync(id, new UpdateOfferRequest { Status = OfferStatus.Withdrawn }, userId);

        if (offer == null)
            return NotFound(new ApiResponse { Success = false, Message = "Offer not found" });

        return Ok(new ApiResponse<Offer>
        {
            Success = true,
            Message = "Offer withdrawn successfully",
            Data = offer
        });
    }
}

