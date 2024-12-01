namespace MyTicket.Domain.Entities.Transaction;

public class Event : BaseEntity
{
    public Guid EventId { get; set; } = new Guid();
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int TotalTicket { get; set; }
    public decimal Price { get; set; }
    public string Location { get; set; }
    public ICollection<OrderTicket>? OrderTickets { get; set; }
}