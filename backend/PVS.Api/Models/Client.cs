using PVS.Api.Modules.Clients.Enums;

namespace PVS.Api.Models;

public class Client
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public Address? Address { get; set; } 
    public ClientType? ClientType { get; set; } 
    public DateTime? DateOfBirth { get; set; }
    public ClientStatus? Status { get; set; } = ClientStatus.Active; 
    public int UserId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}