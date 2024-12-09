using MyTicket.Domain.Entities.Transaction;

namespace MyTicket.Application.Models;

public class TransactionModel
{
    public string EventType { get; set; }
    public Guid EventId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int TotalTicket { get; set; }
    public int AvailableTicket { get; set; }
    public decimal Price { get; set; }
    public string Location { get; set; }
    

     public Guid OrderTicketId { get; set; } = new Guid();
    public Guid UserId { get; set; }
    // public Guid EventId { get; set; }
    public string TicketNumber { get; set; }
    public int Quantity { get; set; }
    public DateTime Date { get; set; }
    public bool IsPaid { get; set; } = false;
}