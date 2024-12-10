using MyTicket.Application.Models;
using MyTicket.Domain.Entities;
using MyTicket.Domain.Entities.Payment;

namespace MyTicket.Application.Businesses.Payment.Models;

public class GetPaymentQueryResponse : BaseResponse
{
    public Guid PaymentHistoryId { get; set; }
    public Guid UserId { get; set; }
    public string Email { get; set; }
    public string FullName { get; set; }
    public Guid EventId { get; set; }
    public string EventName { get; set; }
    public Guid OrderTicketId { get; set; }
    public string TicketNumber { get; set; }
    public decimal PricePayment { get; set; }
    public PaymentStatus Status { get; set; }
    public string LinkPayment { get; set; }
}
