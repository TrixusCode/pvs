using PVS.Api.Common.Repository;
using PVS.Api.Data;
using PVS.Api.Models;
using PVS.Api.Modules.Branches.Enums;

namespace PVS.Api.Modules.Branches.Repository;

public interface IBranchRepository : IGenericRepository<Branch>
{
    Task<IEnumerable<Branch>> GetByStatusAsync(BranchStatus status);
    Task<IEnumerable<Branch>> GetByManagerAsync(int managerId);
}

public class BranchRepository : GenericRepository<Branch>, IBranchRepository
{
    private readonly AppDbContext _context;

    public BranchRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

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