using MyTicket.Application.Models;

namespace MyTicket.Application.Businesses.Event.Models;

public class GetEventResponse : BaseResponse
{
    public Guid EventId { get; set; }   
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int AvailableTickets { get; set; }
    public int Price { get; set; }
    public string Location { get; set; }
}
