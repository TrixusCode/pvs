using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PVS.Api.Common;
using PVS.Api.Data;
using PVS.Api.Models;

namespace PVS.Api.Modules.Branches.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BranchController(AppDbContext context) : ControllerBase
{
    [HttpGet]
    public IActionResult GetAll()
    {
        var branches = context.Branches.ToList();
        return Ok(new ApiResponse<List<Branch>>
        {
            Success = true,
            Data = branches
        });
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var branch = context.Branches.Find(id);
        if (branch == null)
            return NotFound(new ApiResponse { Success = false, Message = "Branch not found" });

        return Ok(new ApiResponse<Branch>
        {
            Success = true,
            Data = branch
        });
    }
}
