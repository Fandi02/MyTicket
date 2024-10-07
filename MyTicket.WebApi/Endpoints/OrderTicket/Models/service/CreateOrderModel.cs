namespace MyTicket.WebApi.Endpoints.OrderTicket.Models.Service;

public class CreateOrderModel
{
    public Guid EventId { get; set; }
    public Guid UserId { get; set; }
    public int Quantity { get; set; }
}