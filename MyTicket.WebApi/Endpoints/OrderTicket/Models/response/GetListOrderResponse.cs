namespace MyTicket.WebApi.Endpoints.OrderTicket.Models.Response;

public class GetListOrderResponse
{
    public Guid OrderTicketId { get; set; }
    public Guid UserId { get; set; }
    public Guid EventId { get; set; }
    public string EventName { get; set; }
    public string TicketNumber { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public DateTime Date { get; set; }
}
