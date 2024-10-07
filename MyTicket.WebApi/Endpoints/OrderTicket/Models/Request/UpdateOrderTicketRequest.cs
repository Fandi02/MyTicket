namespace MyTicket.WebApi.Endpoints.OrderTicket.Models.Request;

public class UpdateOrderTicketRequest
{
    public Guid OrderTicketId { get; set; }
    public int Quantity { get; set; }
}
