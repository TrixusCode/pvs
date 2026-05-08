using PVS.Api.Models;
using PVS.Api.Modules.Properties.Dtos;

namespace PVS.Api.Modules.Properties.Mappers;

public static class PropertyMapper
{
    public static PropertyDto ToDto(this Property property)
    {
        return new PropertyDto
        {
            Id = property.Id,
            Title = property.Title,
            PropertyType = property.PropertyType,
            Description = property.Description,
            Address = property.Address,
            City = property.Address.City,
            State = property.Address.State,
            ZipCode = property.Address.ZipCode,
            Price = property.Price,
            Bedrooms = property.Bedrooms,
            Bathrooms = property.Bathrooms,
            SquareFeet = property.SquareFeet,
            ImagePath = property.ImagePath,
            BranchId = property.BranchId,
            UserId = property.UserId,
            CreatedAt = property.CreatedAt,
            UpdatedAt = property.UpdatedAt
        };
    }

    public static Property ToEntity(this CreatePropertyRequest request, int userId)
    {
        return new Property
        {
            Title = request.Title,
            PropertyType = request.PropertyType,
            Description = request.Description,
            Address = new Address
            {
                City = request.City,
                State = request.State,
                ZipCode = request.ZipCode
            },
            Price = request.Price,
            Bedrooms = request.Bedrooms,
            Bathrooms = request.Bathrooms,
            SquareFeet = request.SquareFeet,
            BranchId = request.BranchId,
            UserId = userId,
            CreatedAt = DateTime.UtcNow
        };
    }

    public static void ApplyTo(this UpdatePropertyRequest request, Property property)
    {
        if (request.Title != null) property.Title = request.Title;
        if (request.Description != null) property.Description = request.Description;
        if (request.PropertyType.HasValue) property.PropertyType = request.PropertyType.Value;
        if (request.BranchId.HasValue) property.BranchId = request.BranchId.Value;
        if (request.City != null) property.Address.City = request.City;
        if (request.State != null) property.Address.State = request.State;
        if (request.ZipCode != null) property.Address.ZipCode = request.ZipCode;
        if (request.Price.HasValue) property.Price = request.Price.Value;
        if (request.Bedrooms.HasValue) property.Bedrooms = request.Bedrooms.Value;
        if (request.Bathrooms.HasValue) property.Bathrooms = request.Bathrooms.Value;
        if (request.SquareFeet.HasValue) property.SquareFeet = request.SquareFeet.Value;

        property.UpdatedAt = DateTime.UtcNow;
    }
}
