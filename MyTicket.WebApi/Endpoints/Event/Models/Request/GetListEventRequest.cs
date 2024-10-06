namespace MyTicket.WebApi.Endpoints.Event.Models.Request;

public class GetListEventRequest
{
    public string? Search { get; set; }
    public int? OrderColumn { get; set; }
    public string? OrderType { get; set; }
    public int Page { get; set; } = 1;
    public int Size { get; set; } = 10;
}
