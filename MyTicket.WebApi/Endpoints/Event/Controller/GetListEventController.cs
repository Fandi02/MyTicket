using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyTicket.Application.Businesses.Event.Models;
using MyTicket.Application.Businesses.Event.Queries;
using MyTicket.WebApi.Endpoints.Event.Models.Request;
using Swashbuckle.AspNetCore.Annotations;

namespace MyTicket.WebApi.Endpoints.Event;

public class GetListEventController : BaseEndpoint<GetListEventRequest, GetEventResponse>
{
    private readonly IMediator _mediator;
    public GetListEventController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [Authorize]
    [SwaggerOperation(
        Summary = "Get list event",
        Description = "",
        Tags = new[] { "Event" })
    ]
    [ProducesResponseType(typeof(GetEventResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public override async Task<ActionResult<GetEventResponse>> HandleAsync([FromQuery] GetListEventRequest request, CancellationToken cancellationToken = default)
    {
        try {
            var mediator = _mediator;
            if (mediator is null)
            {
                var logger = HttpContext.RequestServices.GetRequiredService<ILogger<GetListEventController>>();
                logger.LogWarning("Mediator login is null, fallback to GetRequiredService");
                mediator = HttpContext.RequestServices.GetRequiredService<IMediator>();
            }

            var result = await _mediator.Send(new GetListEventQuery 
                                            { 
                                                Search = request.Search, 
                                                Page = request.Page, 
                                                Size = request.Size ,
                                                OrderColumn = request.OrderColumn,
                                                OrderType = request.OrderType
                                            });

            return Ok(result);
        } catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}