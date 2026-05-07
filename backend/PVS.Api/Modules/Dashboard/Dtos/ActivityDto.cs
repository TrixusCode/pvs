namespace PVS.Api.Modules.Dashboard.Dtos;

public class ActivityDto
{
    public string Type { get; set; } = string.Empty; // "property", "client", "appointment", "offer"
    public string Description { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}