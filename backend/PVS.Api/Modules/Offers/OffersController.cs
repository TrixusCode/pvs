using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PVS.Api.Common;
using PVS.Api.Models;

namespace PVS.Api.Modules.Offers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OffersController : ControllerBase
{
    // Mock data for demo
    private static readonly List<Offer> Offers = new()
    {
        new Offer
        {
            Id = 1,
            PropertyId = 1,
            ClientId = 1,
            OfferedPrice = 425000,
            Status = "Pending",
            OfferDate = DateTime.UtcNow.AddDays(-2),
            ExpirationDate = DateTime.UtcNow.AddDays(3),
            OfferType = "Contingent",
            DownPaymentPercent = 20,
            ClosingDaysRequested = 30,
            Contingencies = "Home inspection, appraisal, financing",
            AgentNotes = "Good client, ready to move",
            UserId = 1,
            CreatedAt = DateTime.UtcNow.AddDays(-2)
        },
        new Offer
        {
            Id = 2,
            PropertyId = 2,
            ClientId = 2,
            OfferedPrice = 245000,
            Status = "Accepted",
            OfferDate = DateTime.UtcNow.AddDays(-5),
            ExpirationDate = DateTime.UtcNow.AddDays(2),
            OfferType = "Full Price",
            DownPaymentPercent = 25,
            ClosingDaysRequested = 45,
            Contingencies = "Financing",
            AgentNotes = "Quick closing expected",
            UserId = 1,
            CreatedAt = DateTime.UtcNow.AddDays(-5)
        }
    };

    [HttpGet]
    public IActionResult GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var skip = (page - 1) * pageSize;
        var total = Offers.Count;
        var items = Offers.Skip(skip).Take(pageSize).ToList();

        return Ok(new PaginatedResponse<Offer>
        {
            Data = items,
            CurrentPage = page,
            PageSize = pageSize,
            TotalCount = total
        });
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var offer = Offers.FirstOrDefault(o => o.Id == id);
        if (offer == null)
            return NotFound(new ApiResponse { Success = false, Message = "Offer not found" });

        return Ok(new ApiResponse<Offer>
        {
            Success = true,
            Data = offer
        });
    }

    [HttpPost]
    public IActionResult Create([FromBody] CreateOfferRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var offer = new Offer
        {
            Id = Offers.Max(o => o.Id) + 1,
            PropertyId = request.PropertyId,
            ClientId = request.ClientId,
            OfferedPrice = request.OfferedPrice,
            Status = request.Status ?? "Pending",
            OfferDate = DateTime.UtcNow,
            ExpirationDate = request.ExpirationDate,
            OfferType = request.OfferType ?? "Full Price",
            DownPaymentPercent = request.DownPaymentPercent,
            ClosingDaysRequested = request.ClosingDaysRequested,
            Contingencies = request.Contingencies ?? string.Empty,
            AgentNotes = request.AgentNotes ?? string.Empty,
            UserId = 1, // TODO: Get from authenticated user
            CreatedAt = DateTime.UtcNow
        };

        Offers.Add(offer);
        return CreatedAtAction(nameof(GetById), new { id = offer.Id }, offer);
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, [FromBody] UpdateOfferRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var offer = Offers.FirstOrDefault(o => o.Id == id);
        if (offer == null)
            return NotFound(new ApiResponse { Success = false, Message = "Offer not found" });

        if (request.OfferedPrice > 0) offer.OfferedPrice = request.OfferedPrice;
        if (!string.IsNullOrEmpty(request.Status)) offer.Status = request.Status;
        if (request.ExpirationDate.HasValue) offer.ExpirationDate = request.ExpirationDate;
        if (request.DownPaymentPercent.HasValue) offer.DownPaymentPercent = request.DownPaymentPercent;
        if (request.ClosingDaysRequested.HasValue) offer.ClosingDaysRequested = request.ClosingDaysRequested;
        if (!string.IsNullOrEmpty(request.Contingencies)) offer.Contingencies = request.Contingencies;
        if (!string.IsNullOrEmpty(request.AgentNotes)) offer.AgentNotes = request.AgentNotes;
        offer.UpdatedAt = DateTime.UtcNow;

        return Ok(new ApiResponse<Offer>
        {
            Success = true,
            Message = "Offer updated successfully",
            Data = offer
        });
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var offer = Offers.FirstOrDefault(o => o.Id == id);
        if (offer == null)
            return NotFound(new ApiResponse { Success = false, Message = "Offer not found" });

        Offers.Remove(offer);

        return Ok(new ApiResponse
        {
            Success = true,
            Message = "Offer deleted successfully"
        });
    }

    [HttpGet("property/{propertyId}")]
    public IActionResult GetByPropertyId(int propertyId)
    {
        var offers = Offers
            .Where(o => o.PropertyId == propertyId)
            .OrderByDescending(o => o.OfferDate)
            .ToList();

        return Ok(new ApiResponse<List<Offer>>
        {
            Success = true,
            Data = offers
        });
    }

    [HttpGet("client/{clientId}")]
    public IActionResult GetByClientId(int clientId)
    {
        var offers = Offers
            .Where(o => o.ClientId == clientId)
            .OrderByDescending(o => o.OfferDate)
            .ToList();

        return Ok(new ApiResponse<List<Offer>>
        {
            Success = true,
            Data = offers
        });
    }

    [HttpGet("by-status/{status}")]
    public IActionResult GetByStatus(string status)
    {
        var offers = Offers
            .Where(o => o.Status.Equals(status, StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(o => o.OfferDate)
            .ToList();

        return Ok(new ApiResponse<List<Offer>>
        {
            Success = true,
            Data = offers
        });
    }

    [HttpPost("{id}/accept")]
    public IActionResult AcceptOffer(int id)
    {
        var offer = Offers.FirstOrDefault(o => o.Id == id);
        if (offer == null)
            return NotFound(new ApiResponse { Success = false, Message = "Offer not found" });

        offer.Status = "Accepted";
        offer.UpdatedAt = DateTime.UtcNow;

        return Ok(new ApiResponse<Offer>
        {
            Success = true,
            Message = "Offer accepted successfully",
            Data = offer
        });
    }

    [HttpPost("{id}/reject")]
    public IActionResult RejectOffer(int id)
    {
        var offer = Offers.FirstOrDefault(o => o.Id == id);
        if (offer == null)
            return NotFound(new ApiResponse { Success = false, Message = "Offer not found" });

        offer.Status = "Rejected";
        offer.UpdatedAt = DateTime.UtcNow;

        return Ok(new ApiResponse<Offer>
        {
            Success = true,
            Message = "Offer rejected successfully",
            Data = offer
        });
    }

    [HttpPost("{id}/withdraw")]
    public IActionResult WithdrawOffer(int id)
    {
        var offer = Offers.FirstOrDefault(o => o.Id == id);
        if (offer == null)
            return NotFound(new ApiResponse { Success = false, Message = "Offer not found" });

        offer.Status = "Withdrawn";
        offer.UpdatedAt = DateTime.UtcNow;

        return Ok(new ApiResponse<Offer>
        {
            Success = true,
            Message = "Offer withdrawn successfully",
            Data = offer
        });
    }

    [HttpGet("price-range")]
    public IActionResult GetByPriceRange([FromQuery] decimal minPrice, [FromQuery] decimal maxPrice)
    {
        var offers = Offers
            .Where(o => o.OfferedPrice >= minPrice && o.OfferedPrice <= maxPrice)
            .OrderByDescending(o => o.OfferedPrice)
            .ToList();

        return Ok(new ApiResponse<List<Offer>>
        {
            Success = true,
            Data = offers
        });
    }
}

public class CreateOfferRequest
{
    public int PropertyId { get; set; }
    public int ClientId { get; set; }
    public decimal OfferedPrice { get; set; }
    public string? Status { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public string? OfferType { get; set; }
    public decimal? DownPaymentPercent { get; set; }
    public int? ClosingDaysRequested { get; set; }
    public string? Contingencies { get; set; }
    public string? AgentNotes { get; set; }
}

public class UpdateOfferRequest
{
    public decimal OfferedPrice { get; set; }
    public string? Status { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public decimal? DownPaymentPercent { get; set; }
    public int? ClosingDaysRequested { get; set; }
    public string? Contingencies { get; set; }
    public string? AgentNotes { get; set; }
}
