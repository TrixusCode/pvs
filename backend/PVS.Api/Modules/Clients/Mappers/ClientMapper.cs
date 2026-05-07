using PVS.Api.Models;
using PVS.Api.Modules.Clients.Dtos;
using PVS.Api.Modules.Clients.Enums;

namespace PVS.Api.Modules.Clients.Mappers;

public static class ClientMapper
{
    public static ClientDto ToDto(this Client client)
    {
        return new ClientDto
        {
            Id = client.Id,
            FirstName = client.FirstName,
            LastName = client.LastName,
            Email = client.Email,
            Phone = client.Phone,
            Address = client.Address,
            ClientType = client.ClientType,
            Status = client.Status,
            UserId = client.UserId,
            CreatedAt = client.CreatedAt,
            UpdatedAt = client.UpdatedAt
        };
    }

    public static Client ToEntity(this CreateClientRequest request, int userId)
    {
        return new Client
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Phone = request.Phone,
            Address = request.Address,
            ClientType = request.ClientType ?? ClientType.Buyer,
            Status = request.Status ?? ClientStatus.Active,
            UserId = userId,
            CreatedAt = DateTime.UtcNow
        };
    }

    public static void ApplyTo(this UpdateClientRequest request, Client client)
    {
        if (request.FirstName != null) client.FirstName = request.FirstName;
        if (request.LastName != null) client.LastName = request.LastName;
        if (request.Email != null) client.Email = request.Email;
        if (request.Phone != null) client.Phone = request.Phone;
        if (request.Address != null) client.Address = request.Address;
        if (request.ClientType != null) client.ClientType = request.ClientType;
        if (request.Status != null) client.Status = request.Status;

        client.UpdatedAt = DateTime.UtcNow;
    }
}
