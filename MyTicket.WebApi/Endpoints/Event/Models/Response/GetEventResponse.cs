namespace MyTicket.WebApi.Endpoints.Event.Models.Response;

public class GetEventResponse
{
    public Guid EventId { get; set; }   
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int AvailableTickets { get; set; }
    public int Price { get; set; }
    public string Location { get; set; }
    
    public string? CreatedBy { get; set; }
    public string? CreatedByName { get; set; }
    public string? CreatedByFullName { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? CreatedAtServer { get; set; }

    public string? LastUpdatedBy { get; set; }
    public string? LastUpdatedByName { get; set; }
    public string? LastUpdatedByFullName { get; set; }
    public DateTime? LastUpdatedAt { get; set; }
    public DateTime? LastUpdatedAtServer { get; set; }
}
