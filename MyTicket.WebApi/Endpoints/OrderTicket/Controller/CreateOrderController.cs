using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyTicket.Application.Constant;
using MyTicket.Application.Exceptions;
using MyTicket.Domain.Entities;
using MyTicket.WebApi.Endpoints.OrderTicket.Models.Request;
using MyTicket.WebApi.ServiceMessageBroker;
using Swashbuckle.AspNetCore.Annotations;

namespace MyTicket.WebApi.Endpoints.OrderTicket;

public class CreateOrderController : BaseEndpointWithoutResponse<CreateOrderTicketRequest>
{
    private readonly IMediator _mediator;
    public CreateOrderController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [Authorize]
    [SwaggerOperation(
        Summary = "Create order ticket for user only",
        Description = "",
        Tags = new[] { "OrderTicket" })
    ]
     [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public override async Task<ActionResult> HandleAsync([FromBody] CreateOrderTicketRequest request, CancellationToken cancellationToken = default)
    {
        try {
            var userId = User.Claims.FirstOrDefault(x => x.Type == ApplicationClaimConstant.UserId)?.Value;
            var role = User.Claims.FirstOrDefault(x => x.Type == ApplicationClaimConstant.Role)?.Value;

            if (userId == null)
                throw new BadRequestException("User not found");

            if (role != UserRoleEnum.User.ToString())
                throw new BadRequestException("Only user can create order ticket");

            var mediator = _mediator;
            if (mediator is null)
            {
                var logger = HttpContext.RequestServices.GetRequiredService<ILogger<CreateOrderController>>();
                logger.LogWarning("Mediator login is null, fallback to GetRequiredService");
                mediator = HttpContext.RequestServices.GetRequiredService<IMediator>();
            }

            var createOrder = new
            {
                EventId = request.EventId,
                UserId = userId,
                Quantity = request.Quantity
            };

            var producer = new MessageProducer();
            producer.SendingMessage("create-order", createOrder);

            return Ok();
        } catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}