using Microsoft.AspNetCore.Http;
using PVS.Api.Models;
using PVS.Api.Modules.Employees.Dtos;

namespace PVS.Api.Modules.Employees.Services;

public interface IEmployeeService
{
    Task<IEnumerable<Employee>> GetAllAsync(int page = 1, int pageSize = 10, string? search = null, int? branchId = null, string? role = null);
    Task<Employee?> GetByIdAsync(int id);
    Task<IEnumerable<Employee>> GetByBranchAsync(int branchId);
    Task<IEnumerable<Employee>> GetByUserAsync(int userId);
    Task<Employee> CreateAsync(CreateEmployeeRequest request);
    Task<Employee?> UpdateAsync(int id, UpdateEmployeeRequest request);
    Task<bool> DeleteAsync(int id);
    Task<Employee?> UploadImageAsync(int employeeId, IFormFile file);
    Task<int> GetTotalCountAsync(string? search = null, int? branchId = null, string? role = null);
}
