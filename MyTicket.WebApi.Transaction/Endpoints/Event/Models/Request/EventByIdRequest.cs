using Microsoft.AspNetCore.Mvc;

namespace MyTicket.WebApi.Transaction.Endpoints.Event.Models.Request;

public class EventByIdRequest
{
    [FromRoute] public Guid EventId { get; set; }
}
