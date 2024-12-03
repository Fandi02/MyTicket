using MyTicket.Domain.Entities.Auth;

namespace MyTicket.Domain.Entities.Transaction;

public class OrderTicket : BaseEntity
{
    public Guid OrderTicketId { get; set; } = new Guid();
    public Guid UserId { get; set; }
    public Guid EventId { get; set; }
    public string TicketNumber { get; set; }
    public int Quantity { get; set; }
    public DateTime Date { get; set; }
    public bool IsPaid { get; set; } = false;
    public Event Event { get; set; } = null!;
}