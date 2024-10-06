using Microsoft.AspNetCore.Mvc;

namespace MyTicket.WebApi.Endpoints.Event.Models.Request;

public class EventByIdRequest
{
    [FromRoute] public Guid EventId { get; set; }
}
