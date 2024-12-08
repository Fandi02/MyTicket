namespace MyTicket.WebApi.Transaction.Endpoints.Event.Models.Request;

public class UpdateEventRequest
{
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int TotalTicket { get; set; }
    public decimal Price { get; set; }
    public string Location { get; set; }
}
