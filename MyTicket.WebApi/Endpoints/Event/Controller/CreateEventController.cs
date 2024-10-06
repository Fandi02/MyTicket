using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyTicket.Application.Businesses.Event.Commands;
using MyTicket.Application.Constant;
using MyTicket.Application.Exceptions;
using MyTicket.Domain.Entities;
using MyTicket.WebApi.Endpoints.Event.Models.Request;
using Swashbuckle.AspNetCore.Annotations;

namespace MyTicket.WebApi.Endpoints.Event;

public class CreateEventController : BaseEndpointWithoutResponse<CreateEventRequest>
{
    private readonly IMediator _mediator;
    public CreateEventController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("create-event")]
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
        try {
            var role = User.Claims.FirstOrDefault(x => x.Type == ApplicationClaimConstant.Role)?.Value;

            if (role != UserRoleEnum.Admin.ToString())
                throw new BadRequestException("Only admin can create event");

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
                                        AvailableTickets = request.AvailableTickets,
                                        Price = request.Price,
                                        Location = request.Location
                                    });

            return Ok();
        } catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}