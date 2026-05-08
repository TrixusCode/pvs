using PVS.Api.Common.Repository;
using PVS.Api.Models;

namespace PVS.Api.Modules.Employees.Repository;

public interface IEmployeeRepository : IGenericRepository<Employee>
{
    Task<IEnumerable<Employee>> GetAllAsync(int page = 1, int pageSize = 10);
    Task<Employee?> GetByIdWithDetailsAsync(int id);
    Task<IEnumerable<Employee>> GetByBranchIdAsync(int branchId);
    Task<IEnumerable<Employee>> GetByUserIdAsync(int userId);
    Task<int> CountByBranchIdAsync(int branchId);
}
