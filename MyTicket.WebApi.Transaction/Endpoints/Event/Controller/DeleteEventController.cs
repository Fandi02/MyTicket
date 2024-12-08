using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyTicket.Application.Businesses.Event.Commands;
using MyTicket.Application.Constant;
using MyTicket.Application.Exceptions;
using MyTicket.Domain.Entities.Auth;
using MyTicket.WebApi.Transaction.Endpoints.Event.Models.Request;
using Swashbuckle.AspNetCore.Annotations;

namespace MyTicket.WebApi.Transaction.Endpoints.Event;

public class DeleteEventController : BaseEndpointWithoutResponse<DeleteEventRequest>
{
    private readonly IMediator _mediator;
    public DeleteEventController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpDelete("{EventId}")]
    [Authorize]
    [SwaggerOperation(
        Summary = "Delete event",
        Description = "",
        Tags = new[] { "Event" })
    ]
     [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public override async Task<ActionResult> HandleAsync([FromRoute] DeleteEventRequest request, CancellationToken cancellationToken = default)
    {
        var role = User.Claims.FirstOrDefault(x => x.Type == ApplicationClaimConstant.Role)?.Value;

        if (role != UserRoleEnum.Admin.ToString())
            throw new BadRequestException("Only admin can create event");
                
        var mediator = _mediator;
        if (mediator is null)
        {
            var logger = HttpContext.RequestServices.GetRequiredService<ILogger<DeleteEventRequest>>();
            logger.LogWarning("Mediator login is null, fallback to GetRequiredService");
            mediator = HttpContext.RequestServices.GetRequiredService<IMediator>();
        }

        await _mediator.Send(new DeleteEventCommand
        { 
            EventId = request.EventId
        });

        return Ok();
    }
}