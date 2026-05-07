using PVS.Api.Common.Repository;
using PVS.Api.Models;
using PVS.Api.Modules.Branches.Enums;

namespace PVS.Api.Modules.Branches.Repository;

public interface IBranchRepository : IGenericRepository<Branch>
{
    Task<IEnumerable<Branch>> GetByStatusAsync(BranchStatus status);
    Task<IEnumerable<Branch>> GetByManagerAsync(int managerId);
}