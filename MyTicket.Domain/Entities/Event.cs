namespace MyTicket.Domain.Entities;

public class Event : BaseEntity
{
    public Guid EventId { get; set; } = new Guid();
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int AvailableTickets { get; set; }
    public int Price { get; set; }
    public string Location { get; set; }
}