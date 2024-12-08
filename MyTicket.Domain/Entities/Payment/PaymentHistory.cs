using MyTicket.Domain.Entities.Auth;
using MyTicket.Domain.Entities.Transaction;

namespace MyTicket.Domain.Entities.Payment
{
    public enum PaymentStatus
    {
        WaitingForPayment,
        PaymentComplete
    }

    public class PaymentHistory : BaseEntity
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid OrderTicketId { get; set; }
        public decimal PricePayment { get; set; }
        public PaymentStatus Status { get; set; }
        public string? Token { get; set; }
        public string? RedirectUrl { get; set; }
        public User User { get; set; }
        public OrderTicket OrderTicket { get; set; }
    }
}
