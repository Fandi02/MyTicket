using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project.Application.Businesses.Payment.Commands;
using Swashbuckle.AspNetCore.Annotations;

namespace MyTicket.WebApi.Endpoints.Payment;

public class VerifyPaymentMidtransController : BaseEndpoint<VerifyMidtransPaymentCommand, string>
{
    private IMediator _mediator;
    public VerifyPaymentMidtransController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [AllowAnonymous]
    [HttpPost("verify-payment")]
    [SwaggerOperation(
        Summary = "Verify Payment Midtrans",
        Description = "",
        Tags = new[] { "Payment" })
    ]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public override async Task<ActionResult<string>> HandleAsync([FromBody] VerifyMidtransPaymentCommand request, CancellationToken cancellationToken = default)
    {
        var responsePaymentHistory = await _mediator.Send(request);

        return responsePaymentHistory;
    }
}