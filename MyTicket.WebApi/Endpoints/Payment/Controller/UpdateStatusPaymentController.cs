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

public class UpdateStatusPaymentController : BaseEndpointWithoutResponse<UpdateStatusPaymentRequest>
{
    private readonly IMediator _mediator;
    public UpdateStatusPaymentController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPut("update-status")]
    [Authorize]
    [SwaggerOperation(
        Summary = "Update payment",
        Description = "",
        Tags = new[] { "Payment" })
    ]
     [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public override async Task<ActionResult> HandleAsync([FromBody] UpdateStatusPaymentRequest request, CancellationToken cancellationToken = default)
    {
        try {
            var userId = User.Claims.FirstOrDefault(x => x.Type == ApplicationClaimConstant.UserId)?.Value;
            var role = User.Claims.FirstOrDefault(x => x.Type == ApplicationClaimConstant.Role)?.Value;

            if (userId == null)
                throw new BadRequestException("User not found");

            if (role != UserRoleEnum.Admin.ToString())
                throw new BadRequestException("Only admin can update status payment");

            var mediator = _mediator;
            if (mediator is null)
            {
                var logger = HttpContext.RequestServices.GetRequiredService<ILogger<UpdateStatusPaymentController>>();
                logger.LogWarning("Mediator login is null, fallback to GetRequiredService");
                mediator = HttpContext.RequestServices.GetRequiredService<IMediator>();
            }

            await _mediator.Send(new UpdateStatusPaymentCommand 
                                        { 
                                            PaymentId = request.PaymentId,
                                            Status = request.Status,
                                            RejectedReason = !string.IsNullOrEmpty(request.RejectedReason) ? request.RejectedReason : ""
                                        });

            return Ok();
        } catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}