using Microsoft.EntityFrameworkCore;
using PVS.Api.Common.Repository;
using PVS.Api.Data;
using PVS.Api.Models;

namespace PVS.Api.Modules.Employees.Repository;

public class EmployeeRepository(AppDbContext context) : GenericRepository<Employee>(context), IEmployeeRepository
{
    private readonly AppDbContext _context = context;

    public async Task<IEnumerable<Employee>> GetAllAsync(int page = 1, int pageSize = 10)
    {
        var skip = (page - 1) * pageSize;
        return await _context.Employees
            .Include(e => e.Branch)
            .Include(e => e.Address)
            .OrderByDescending(e => e.CreatedAt)
            .Skip(skip)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<Employee?> GetByIdWithDetailsAsync(int id)
    {
        return await _context.Employees
            .Include(e => e.Branch)
            .Include(e => e.Address)
            .Include(e => e.User)
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<IEnumerable<Employee>> GetByBranchIdAsync(int branchId)
    {
        return await _context.Employees
            .Where(e => e.BranchId == branchId)
            .Include(e => e.Address)
            .OrderByDescending(e => e.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Employee>> GetByUserIdAsync(int userId)
    {
        return await _context.Employees
            .Where(e => e.UserId == userId)
            .Include(e => e.Branch)
            .Include(e => e.Address)
            .ToListAsync();
    }

    public async Task<int> CountByBranchIdAsync(int branchId)
    {
        return await _context.Employees.CountAsync(e => e.BranchId == branchId);
    }
}
