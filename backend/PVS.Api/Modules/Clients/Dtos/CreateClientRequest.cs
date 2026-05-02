using PVS.Api.Models;
using PVS.Api.Modules.Clients.Enums;

namespace PVS.Api.Modules.Clients.Dtos;

public class CreateClientRequest
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public Address? Address { get; set; } 
    public ClientType? ClientType { get; set; }
    public ClientStatus? Status { get; set; }
}