namespace MyTicket.WebApi.Endpoints.Payment.Models.Request;

public class UpdatePaymentRequest
{
    public Guid PaymentId { get; set; }
    public string ImagePayment { get; set; }
    public string Description { get; set; }
    public decimal TotalPayment { get; set; }
}