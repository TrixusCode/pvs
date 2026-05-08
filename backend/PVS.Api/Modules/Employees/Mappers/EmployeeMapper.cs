using PVS.Api.Models;
using PVS.Api.Modules.Employees.Dtos;

namespace PVS.Api.Modules.Employees.Mappers;

public static class EmployeeMapper
{
    public static EmployeeDto ToDto(this Employee employee)
    {
        return new EmployeeDto
        {
            Id = employee.Id,
            FirstName = employee.FirstName,
            LastName = employee.LastName,
            PhoneNumber = employee.PhoneNumber,
            Birthdate = employee.Birthdate,
            BranchId = employee.BranchId,
            BranchName = employee.Branch?.Name ?? string.Empty,
            BranchPhone = employee.Branch?.Phone ?? string.Empty,
            BranchEmail = employee.Branch?.Email ?? string.Empty,
            Role = employee.Role,
            IsActive = employee.IsActive,
            IsClient = employee.IsClient,
            ImagePath = employee.ImagePath,
            UserId = employee.UserId,
            Address = employee.Address,
            CreatedAt = employee.CreatedAt,
            ModifiedAt = employee.ModifiedAt
        };
    }

    public static Employee ToEntity(this CreateEmployeeRequest request)
    {
        return new Employee
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            PhoneNumber = request.PhoneNumber,
            Birthdate = request.Birthdate,
            UserId = request.UserId,
            BranchId = request.BranchId,
            Role = request.Role,
            IsActive = request.IsActive,
            IsClient = request.IsClient,
            Address = request.Address,
            CreatedAt = DateTime.UtcNow
        };
    }

    public static void ApplyTo(this UpdateEmployeeRequest request, Employee employee)
    {
        if (request.FirstName != null) employee.FirstName = request.FirstName;
        if (request.LastName != null) employee.LastName = request.LastName;
        if (request.PhoneNumber != null) employee.PhoneNumber = request.PhoneNumber;
        if (request.Birthdate.HasValue) employee.Birthdate = request.Birthdate.Value;
        if (request.BranchId.HasValue) employee.BranchId = request.BranchId.Value;
        if (request.Role.HasValue) employee.Role = request.Role.Value;
        if (request.IsActive.HasValue) employee.IsActive = request.IsActive.Value;
        if (request.IsClient.HasValue) employee.IsClient = request.IsClient.Value;
        if (request.Address != null)
        {
            employee.Address ??= new Address();
            employee.Address.City = request.Address.City;
            employee.Address.State = request.Address.State;
            employee.Address.ZipCode = request.Address.ZipCode;
        }

        employee.ModifiedAt = DateTime.UtcNow;
    }
}
