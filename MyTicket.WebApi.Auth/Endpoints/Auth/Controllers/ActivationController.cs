using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyTicket.Application.Businesses.Auth.Commands;
using MyTicket.Application.Exceptions;
using MyTicket.WebApi.Endpoints;
using Swashbuckle.AspNetCore.Annotations;

namespace MyTicket.WebApi.Auth.Endpoints.Auth;

public class ActivationController : BaseEndpointWithoutResponse<Guid>
{
    private readonly IMediator _mediator;
    public ActivationController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [AllowAnonymous]
    [HttpGet("activation")]
    [SwaggerOperation(
        Summary = "Activation",
        Description = "",
        Tags = new[] { "Auth" })
    ]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public override async Task<ActionResult> HandleAsync([FromQuery] Guid Id, CancellationToken cancellationToken = default)
    {
        if (Id == Guid.Empty)
            throw new BadRequestException("Request is null");

        await _mediator.Send(new ActivationCommand { 
            UserId = Id, 
        });

        return Ok();
    }
}