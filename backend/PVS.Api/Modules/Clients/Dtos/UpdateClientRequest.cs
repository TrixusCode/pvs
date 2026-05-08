using PVS.Api.Models;
using PVS.Api.Modules.Clients.Enums;

namespace PVS.Api.Modules.Clients.Dtos;

public class UpdateClientRequest
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public Address? Address { get; set; }

    public ClientType? ClientType { get; set; }
    public ClientStatus? Status { get; set; }
    public DateTime? DateOfBirth { get; set; }
}
