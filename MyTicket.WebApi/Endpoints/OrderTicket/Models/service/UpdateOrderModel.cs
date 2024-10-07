namespace MyTicket.WebApi.Endpoints.OrderTicket.Models.Service;

public class UpdateOrderModel
{
    public Guid OrderTicketId { get; set; }
    public int Quantity { get; set; }
}