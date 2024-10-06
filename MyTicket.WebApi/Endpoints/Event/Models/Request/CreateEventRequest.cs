namespace MyTicket.WebApi.Endpoints.Event.Models.Request;

public class CreateEventRequest
{
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int AvailableTickets { get; set; }
    public decimal Price { get; set; }
    public string Location { get; set; }
}
