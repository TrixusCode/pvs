using Microsoft.AspNetCore.Http;
using PVS.Api.Models;
using PVS.Api.Modules.Properties.Dtos;

namespace PVS.Api.Modules.Properties.Services;

public interface IPropertiesService
{
    Task<IEnumerable<Property>> GetAllAsync(int page = 1, int pageSize = 10);
    Task<Property?> GetByIdAsync(int id);
    Task<Property> CreateAsync(CreatePropertyRequest request, int userId);
    Task<Property?> UpdateAsync(int id, UpdatePropertyRequest request, int userId);
    Task<bool> DeleteAsync(int id, int userId);
    Task<Property?> UploadImageAsync(int propertyId, IFormFile file);
    Task<int> GetTotalCountAsync();
}