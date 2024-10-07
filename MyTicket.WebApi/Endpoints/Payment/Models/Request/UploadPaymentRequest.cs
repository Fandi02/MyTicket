namespace MyTicket.WebApi.Endpoints.Payment.Models.Request;

public class UploadPaymentRequest
{
    public Guid OrderTicketId { get; set; }
    public string ImagePayment { get; set; }
    public string Description { get; set; }
    public decimal TotalPayment { get; set; }
}