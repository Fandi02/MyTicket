using MyTicket.Domain.Entities;

namespace MyTicket.WebApi.Endpoints.Payment.Models.Request;

public class UpdateStatusPaymentRequest
{
    public Guid PaymentId { get; set; }
    public StatusPaymentEnum Status { get; set; }
    public string? RejectedReason { get; set; }
}