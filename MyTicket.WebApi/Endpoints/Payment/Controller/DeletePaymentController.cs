using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyTicket.Application.Businesses.OrderTicket.Commands;
using MyTicket.Application.Constant;
using MyTicket.Application.Exceptions;
using MyTicket.WebApi.Endpoints.Payment.Models.Request;
using Swashbuckle.AspNetCore.Annotations;

namespace MyTicket.WebApi.Endpoints.Payment;

public class DeletePaymentController : BaseEndpointWithoutResponse<DeletePaymentRequest>
{
    private readonly IMediator _mediator;
    public DeletePaymentController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpDelete]
    [Authorize]
    [SwaggerOperation(
        Summary = "Delete payment",
        Description = "",
        Tags = new[] { "Payment" })
    ]
     [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public override async Task<ActionResult> HandleAsync([FromQuery] DeletePaymentRequest request, CancellationToken cancellationToken = default)
    {
        try {
            var userId = User.Claims.FirstOrDefault(x => x.Type == ApplicationClaimConstant.UserId)?.Value;
            var role = User.Claims.FirstOrDefault(x => x.Type == ApplicationClaimConstant.Role)?.Value;

            if (userId == null)
                throw new BadRequestException("User not found");

            var mediator = _mediator;
            if (mediator is null)
            {
                var logger = HttpContext.RequestServices.GetRequiredService<ILogger<DeletePaymentController>>();
                logger.LogWarning("Mediator login is null, fallback to GetRequiredService");
                mediator = HttpContext.RequestServices.GetRequiredService<IMediator>();
            }

            await _mediator.Send(new DeletePaymentCommand 
                                        { 
                                            PaymentId = request.PaymentId
                                        });

            return Ok();
        } catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}