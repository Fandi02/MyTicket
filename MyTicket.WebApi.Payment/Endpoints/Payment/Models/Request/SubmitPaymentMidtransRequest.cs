namespace MyTicket.WebApi.Endpoints.Payment.Models.Request;

public class SubmitPaymentMidtransRequest
{
    public Guid OrderTicketId { get; set; }
    public decimal PricePayment { get; set; }
}
