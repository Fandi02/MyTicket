using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyTicket.Application.Businesses.OrderTicket.Commands;
using MyTicket.Application.Constant;
using MyTicket.Application.Exceptions;
using MyTicket.Domain.Entities;
using MyTicket.WebApi.Endpoints.Payment.Models.Request;
using Swashbuckle.AspNetCore.Annotations;

namespace MyTicket.WebApi.Endpoints.Payment;

public class UpdatePaymentController : BaseEndpointWithoutResponse<UpdatePaymentRequest>
{
    private readonly IMediator _mediator;
    public UpdatePaymentController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPut]
    [Authorize]
    [SwaggerOperation(
        Summary = "Update payment",
        Description = "",
        Tags = new[] { "Payment" })
    ]
     [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public override async Task<ActionResult> HandleAsync([FromBody] UpdatePaymentRequest request, CancellationToken cancellationToken = default)
    {
        try {
            var userId = User.Claims.FirstOrDefault(x => x.Type == ApplicationClaimConstant.UserId)?.Value;
            var role = User.Claims.FirstOrDefault(x => x.Type == ApplicationClaimConstant.Role)?.Value;

            if (userId == null)
                throw new BadRequestException("User not found");

            if (role != UserRoleEnum.User.ToString())
                throw new BadRequestException("Only user can update payment");

            var mediator = _mediator;
            if (mediator is null)
            {
                var logger = HttpContext.RequestServices.GetRequiredService<ILogger<UpdatePaymentController>>();
                logger.LogWarning("Mediator login is null, fallback to GetRequiredService");
                mediator = HttpContext.RequestServices.GetRequiredService<IMediator>();
            }

            await _mediator.Send(new UpdatePaymentCommand 
                                        { 
                                            PaymentId = request.PaymentId,
                                            ImagePayment = request.ImagePayment,
                                            Description = request.Description,
                                            TotalPayment = request.TotalPayment
                                        });

            return Ok();
        } catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}