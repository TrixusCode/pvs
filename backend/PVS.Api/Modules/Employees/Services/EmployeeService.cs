using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using PVS.Api.Data;
using PVS.Api.Models;
using PVS.Api.Modules.Employees.Dtos;
using PVS.Api.Modules.Employees.Enums;
using PVS.Api.Modules.Employees.Mappers;
using PVS.Api.Modules.Employees.Repository;

namespace PVS.Api.Modules.Employees.Services;

public class EmployeeService(
    IEmployeeRepository employeeRepository,
    AppDbContext context,
    IWebHostEnvironment environment)
    : IEmployeeService
{
    public async Task<IEnumerable<Employee>> GetAllAsync(int page = 1, int pageSize = 10, string? search = null, int? branchId = null, string? role = null)
    {
        var skip = (page - 1) * pageSize;
        return await BuildFilteredQuery(search, branchId, role)
            .OrderByDescending(e => e.CreatedAt)
            .Skip(skip)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<Employee?> GetByIdAsync(int id)
    {
        return await employeeRepository.GetByIdWithDetailsAsync(id);
    }

    public async Task<IEnumerable<Employee>> GetByBranchAsync(int branchId)
    {
        return await employeeRepository.GetByBranchIdAsync(branchId);
    }

    public async Task<IEnumerable<Employee>> GetByUserAsync(int userId)
    {
        return await employeeRepository.GetByUserIdAsync(userId);
    }

    public async Task<Employee> CreateAsync(CreateEmployeeRequest request)
    {
        var branch = await context.Branches.FindAsync(request.BranchId);
        if (branch == null)
            throw new InvalidOperationException("Branch not found");

        var branchEmployeeCount = await employeeRepository.CountByBranchIdAsync(request.BranchId);
        if (branchEmployeeCount >= Branch.MaxEmployees)
            throw new InvalidOperationException($"Branch has reached maximum staffing capacity ({Branch.MaxEmployees})");

        var employee = request.ToEntity();
        await employeeRepository.AddAsync(employee);
        return employee;
    }

    public async Task<Employee?> UpdateAsync(int id, UpdateEmployeeRequest request)
    {
        var employee = await employeeRepository.GetByIdWithDetailsAsync(id);
        if (employee == null)
            return null;

        if (request.BranchId.HasValue)
        {
            var branch = await context.Branches.FindAsync(request.BranchId.Value);
            if (branch == null)
                throw new InvalidOperationException("Branch not found");

            var branchEmployeeCount = await employeeRepository.CountByBranchIdAsync(request.BranchId.Value);
            if (branchEmployeeCount >= Branch.MaxEmployees && employee.BranchId != request.BranchId.Value)
                throw new InvalidOperationException($"Branch has reached maximum staffing capacity ({Branch.MaxEmployees})");
        }

        request.ApplyTo(employee);
        await employeeRepository.UpdateAsync(employee);

        return employee;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var employee = await employeeRepository.GetByIdWithDetailsAsync(id);
        if (employee == null)
            return false;

        await employeeRepository.DeleteAsync(employee);
        return true;
    }

    public async Task<Employee?> UploadImageAsync(int employeeId, IFormFile? file)
    {
        var employee = await employeeRepository.GetByIdWithDetailsAsync(employeeId);
        if (employee == null || file == null || file.Length == 0)
            return null;

        var uploadsRoot = Path.Combine(environment.ContentRootPath, "wwwroot", "uploads", "employees");
        Directory.CreateDirectory(uploadsRoot);

        var fileName = Path.GetFileName(file.FileName);
        var uniqueName = $"{Guid.NewGuid()}_{fileName}";
        var filePath = Path.Combine(uploadsRoot, uniqueName);

        await using var stream = File.Create(filePath);
        await file.CopyToAsync(stream);

        employee.ImagePath = $"/uploads/employees/{uniqueName}";
        await employeeRepository.UpdateAsync(employee);
        return employee;
    }

    public async Task<int> GetTotalCountAsync(string? search = null, int? branchId = null, string? role = null)
    {
        return await BuildFilteredQuery(search, branchId, role).CountAsync();
    }

    private IQueryable<Employee> BuildFilteredQuery(string? search, int? branchId, string? role)
    {
        var query = context.Employees
            .Include(e => e.Branch)
            .Include(e => e.Address)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var normalizedSearch = search.Trim();
            query = query.Where(e =>
                e.FirstName.Contains(normalizedSearch) ||
                e.LastName.Contains(normalizedSearch) ||
                e.PhoneNumber.Contains(normalizedSearch) ||
                e.Branch.Name.Contains(normalizedSearch));
        }

        if (branchId.HasValue)
        {
            query = query.Where(e => e.BranchId == branchId.Value);
        }

        if (!string.IsNullOrWhiteSpace(role) && Enum.TryParse<EmployeeRole>(role, out var parsedRole))
        {
            query = query.Where(e => e.Role == parsedRole);
        }

        return query;
    }
}
