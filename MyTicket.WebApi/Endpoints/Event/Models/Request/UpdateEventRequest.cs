using Microsoft.AspNetCore.Mvc;

namespace MyTicket.WebApi.Endpoints.Event.Models.Request;

public class UpdateEventRequest
{
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int AvailableTickets { get; set; }
    public int Price { get; set; }
    public string Location { get; set; }
}
