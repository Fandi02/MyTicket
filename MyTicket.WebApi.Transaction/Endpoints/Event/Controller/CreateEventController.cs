using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyTicket.Application.Businesses.Event.Commands;
using MyTicket.Application.Businesses.Event.Queries;
using MyTicket.Application.Constant;
using MyTicket.Application.Exceptions;
using MyTicket.Domain.Entities.Auth;
using MyTicket.WebApi.Transaction.Endpoints.Event.Models.Request;
using Swashbuckle.AspNetCore.Annotations;

namespace MyTicket.WebApi.Transaction.Endpoints.Event;

public class CreateEventController : BaseEndpointWithoutResponse<CreateEventRequest>
{
    private readonly IMediator _mediator;
    public CreateEventController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [Authorize]
    [SwaggerOperation(
        Summary = "Create event",
        Description = "",
        Tags = new[] { "Event" })
    ]
     [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public override async Task<ActionResult> HandleAsync([FromBody] CreateEventRequest request, CancellationToken cancellationToken = default)
    {
        var role = User.Claims.FirstOrDefault(x => x.Type == ApplicationClaimConstant.Role)?.Value;

        if (role != UserRoleEnum.Admin.ToString())
            throw new ForbiddenException("Only admin can create event");

        var mediator = _mediator;
        if (mediator is null)
        {
            var logger = HttpContext.RequestServices.GetRequiredService<ILogger<CreateEventController>>();
            logger.LogWarning("Mediator login is null, fallback to GetRequiredService");
            mediator = HttpContext.RequestServices.GetRequiredService<IMediator>();
        }

        await _mediator.Send(new CreateEventCommand
        { 
            Name = request.Name,
            Description = request.Description,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            TotalTicket = request.TotalTicket,
            Price = request.Price,
            Location = request.Location
        });

        return Ok();
    }
}