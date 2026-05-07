namespace PVS.Api.Modules.Dashboard.Dtos;

public class DashboardStatisticsDto
{
    public int TotalProperties { get; set; }
    public int TotalClients { get; set; }
    public int ActiveAppointments { get; set; }
    public int PendingOffers { get; set; }
}