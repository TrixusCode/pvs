namespace PVS.Api.Modules.Clients.Dtos;

using PVS.Api.Models;
using PVS.Api.Modules.Clients.Enums;

public class ClientDto
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public Address? Address { get; set; }
    public ClientType? ClientType { get; set; }
    public ClientStatus? Status { get; set; }
    public int UserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
