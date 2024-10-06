namespace MyTicket.Domain.Entities;

public class OrderTicket : BaseEntity
{
    public Guid OrderTicketId { get; set; } = new Guid();
    public Guid UserId { get; set; }
    public User User { get; set; }
    public Guid EventId { get; set; }
    public Event Event { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public DateTime Date { get; set; }
}