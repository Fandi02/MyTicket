using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyTicket.Application.Businesses.Event.Models;
using MyTicket.Application.Businesses.Event.Queries;
using MyTicket.WebApi.Endpoints.Event.Models.Request;
using Swashbuckle.AspNetCore.Annotations;

namespace MyTicket.WebApi.Endpoints.Event;

public class GetEventByIdController : BaseEndpoint<EventByIdRequest, GetEventResponse>
{
    private readonly IMediator _mediator;
    public GetEventByIdController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{EventId}")]
    [Authorize]
    [SwaggerOperation(
        Summary = "Get event by id",
        Description = "",
        Tags = new[] { "Event" })
    ]
    [ProducesResponseType(typeof(GetEventResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public override async Task<ActionResult<GetEventResponse>> HandleAsync(EventByIdRequest request, CancellationToken cancellationToken = default)
    {
        try {
            var mediator = _mediator;
            if (mediator is null)
            {
                var logger = HttpContext.RequestServices.GetRequiredService<ILogger<GetEventByIdController>>();
                logger.LogWarning("Mediator login is null, fallback to GetRequiredService");
                mediator = HttpContext.RequestServices.GetRequiredService<IMediator>();
            }

            var result = await _mediator.Send(new GetEventByIdQuery 
                                            { 
                                                EventId = request.EventId
                                            });

            return Ok(result);
        } catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}