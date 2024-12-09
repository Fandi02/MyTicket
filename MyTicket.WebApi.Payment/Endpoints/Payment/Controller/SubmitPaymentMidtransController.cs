using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyTicket.Application.Constant;
using MyTicket.Application.Exceptions;
using MyTicket.Domain.Entities.Auth;
using MyTicket.WebApi.Endpoints.Payment.Models.Request;
using Project.Application.Businesses.Payment.Commands;
using Swashbuckle.AspNetCore.Annotations;

namespace MyTicket.WebApi.Endpoints.Payment;

public class SubmitPaymentMidtransController : BaseEndpoint<SubmitPaymentMidtransRequest, string>
{
    private IMediator _mediator;
    public SubmitPaymentMidtransController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("submit-payment")]
    [Authorize]
    [SwaggerOperation(
        Summary = "Submit Payment Midtrans",
        Description = "",
        Tags = new[] { "Payment" })
    ]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public override async Task<ActionResult<string>> HandleAsync([FromBody] SubmitPaymentMidtransRequest request, CancellationToken cancellationToken = default)
    {
        if (request is null)
            throw new BadRequestException("Request is null");

        var userId = User.Claims.FirstOrDefault(x => x.Type == ApplicationClaimConstant.UserId)?.Value;
        var role = User.Claims.FirstOrDefault(x => x.Type == ApplicationClaimConstant.Role)?.Value;

        if (userId is null)
            throw new BadRequestException("Request is null");

        if (role != UserRoleEnum.User.ToString())
            throw new BadRequestException("Only user can create payment");

        var responsePaymentHistory = await _mediator.Send( new CreatePaymentHistoryCommand 
        { 
            UserId = Guid.Parse(userId),
            OrderTicketId = request.OrderTicketId,
            PricePayment = request.PricePayment
        });

        var response = await _mediator.Send(new CreateTokenLinkMidtransCommand
        {
            OrderId = responsePaymentHistory
        });

        return response.redirect_url;
    }
}