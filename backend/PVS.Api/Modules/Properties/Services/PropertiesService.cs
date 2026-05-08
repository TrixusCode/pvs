using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using PVS.Api.Data;
using PVS.Api.Models;
using PVS.Api.Modules.Properties.Dtos;
using PVS.Api.Modules.Properties.Mappers;

namespace PVS.Api.Modules.Properties.Services;

public class PropertiesService : IPropertiesService
{
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _environment;

    public PropertiesService(AppDbContext context, IWebHostEnvironment environment)
    {
        _context = context;
        _environment = environment;
    }

    public async Task<IEnumerable<Property>> GetAllAsync(int page = 1, int pageSize = 10)
    {
        var skip = (page - 1) * pageSize;
        return await Task.FromResult(_context.Properties
            .Include(p => p.Address)
            .Include(p => p.Branch)
            .OrderByDescending(p => p.CreatedAt)
            .Skip(skip)
            .Take(pageSize)
            .ToList());
    }

    public async Task<Property?> GetByIdAsync(int id)
    {
        return await _context.Properties
            .Include(p => p.Address)
            .Include(p => p.Branch)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Property> CreateAsync(CreatePropertyRequest request, int userId)
    {
        var branch = await _context.Branches.FindAsync(request.BranchId);
        if (branch == null)
            throw new InvalidOperationException("Branch not found");

        var property = request.ToEntity(userId);

        _context.Properties.Add(property);
        await _context.SaveChangesAsync();

        return property;
    }

    public async Task<Property?> UpdateAsync(int id, UpdatePropertyRequest request, int userId)
    {
        var property = await GetByIdAsync(id);
        if (property == null || property.UserId != userId)
            return null;

        if (request.BranchId.HasValue)
        {
            var branch = await _context.Branches.FindAsync(request.BranchId.Value);
            if (branch == null)
                throw new InvalidOperationException("Branch not found");
        }

        request.ApplyTo(property);

        await _context.SaveChangesAsync();
        return property;
    }

    public async Task<bool> DeleteAsync(int id, int userId)
    {
        var property = await GetByIdAsync(id);
        if (property == null || property.UserId != userId)
            return false;

        _context.Properties.Remove(property);
        await _context.SaveChangesAsync();
        return true;
    }
    /// This if for image upload
    public async Task<Property?> UploadImageAsync(int propertyId, IFormFile? file)
    {
        var property = await GetByIdAsync(propertyId);
        if (property == null || file == null || file.Length == 0)
            return null;

        var uploadsRoot = Path.Combine(_environment.ContentRootPath, "wwwroot", "uploads", "properties");
        Directory.CreateDirectory(uploadsRoot);

        var fileName = Path.GetFileName(file.FileName);
        var uniqueName = $"{Guid.NewGuid()}_{fileName}";
        var filePath = Path.Combine(uploadsRoot, uniqueName);

        await using var stream = File.Create(filePath);
        await file.CopyToAsync(stream);

        property.ImagePath = $"/uploads/properties/{uniqueName}";
        await _context.SaveChangesAsync();
        return property;
    }

    public async Task<int> GetTotalCountAsync()
    {
        return await Task.FromResult(_context.Properties.Count());
    }
}
