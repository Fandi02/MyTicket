namespace MyTicket.WebApi.Endpoints.Payment.Models.Service;

public class PaymentConsumerModel
{
    public string Email { get; set; }
    public string FullName { get; set; }
    public string EventName { get; set; }
    public string TicketNumber { get; set; }
    public string? RejectedReason { get; set; }
}