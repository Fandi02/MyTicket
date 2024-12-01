namespace MyTicket.Domain.Entities;

public enum StatusPaymentEnum
{
    Received,
    Approved,
    Rejected
}

public class Payment : BaseEntity
{
    public Guid PaymentId { get; set; }
    public string ImagePayment { get; set; }
    public string Description { get; set; }
    public decimal TotalPayment { get; set; }
    public DateTime Date { get; set; }
    public StatusPaymentEnum Status { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; }
    public Guid OrderTicketId { get; set; }
    public OrderTicket OrderTicket { get; set; }
    public string? RejectedReason { get; set; } = "";
}