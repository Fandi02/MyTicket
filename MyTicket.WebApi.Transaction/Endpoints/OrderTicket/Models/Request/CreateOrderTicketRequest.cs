namespace MyTicket.WebApi.Transaction.Endpoints.OrderTicket.Models.Request;

public class CreateOrderTicketRequest
{
    public Guid EventId { get; set; }
    public int Quantity { get; set; }
}
