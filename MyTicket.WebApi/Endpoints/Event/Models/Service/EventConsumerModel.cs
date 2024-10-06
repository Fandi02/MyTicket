namespace MyTicket.WebApi.Endpoints.Event.Models.Service;

public class EventConsumerModel
{
    public string Email { get; set; }
    public string FullName { get; set; }
    public string EventName { get; set; }
    public string Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Location { get; set; }
}