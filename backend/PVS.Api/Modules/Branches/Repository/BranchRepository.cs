using PVS.Api.Common.Repository;
using PVS.Api.Data;
using PVS.Api.Models;
using PVS.Api.Modules.Branches.Enums;

namespace PVS.Api.Modules.Branches.Repository;

public class BranchRepository(AppDbContext context) : GenericRepository<Branch>(context), IBranchRepository
{
    private readonly AppDbContext _context = context;

    public async Task<IEnumerable<Branch>> GetByStatusAsync(BranchStatus status)
    {
        return await Task.FromResult(_context.Branches
            .Where(b => b.Status == status)
            .ToList());
    }

    public async Task<IEnumerable<Branch>> GetByManagerAsync(int managerId)
    {
        return await Task.FromResult(_context.Branches
            .Where(b => b.ManagerUserId == managerId)
            .ToList());
    }
}