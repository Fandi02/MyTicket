using MyTicket.Application.Models;
using MyTicket.Domain.Entities;

namespace MyTicket.WebApi.Endpoints.Payment.Models.Response;

public class GetPaymentResponse : BaseResponse
{
    public Guid PaymentId { get; set; }
    public Guid EventId { get; set; }
    public string EventName { get; set; }
    public Guid UserId { get; set; }
    public string Email { get; set; }
    public string FullName { get; set; }
    public string ImagePayment { get; set; }
    public string Description { get; set; }
    public decimal TotalPayment { get; set; }
    public DateTime Date { get; set; }
    public StatusPaymentEnum Status { get; set; }
}