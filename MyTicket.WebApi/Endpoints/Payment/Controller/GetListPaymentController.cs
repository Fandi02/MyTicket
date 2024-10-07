using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyTicket.Application.Businesses.Payment.Queries;
using MyTicket.Application.Constant;
using MyTicket.Application.Exceptions;
using MyTicket.Domain.Entities;
using MyTicket.WebApi.Endpoints.Payment.Models.Request;
using MyTicket.WebApi.Endpoints.Payment.Models.Response;
using Swashbuckle.AspNetCore.Annotations;

namespace MyTicket.WebApi.Endpoints.Payment;

public class GetListPaymentController : BaseEndpoint<GetListPaymentRequest, GetPaymentResponse>
{
    private readonly IMediator _mediator;
    public GetListPaymentController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("get-by-user")]
    [Authorize]
    [SwaggerOperation(
        Summary = "Get list payment by user",
        Description = "",
        Tags = new[] { "Payment" })
    ]
    [ProducesResponseType(typeof(GetPaymentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public override async Task<ActionResult<GetPaymentResponse>> HandleAsync([FromQuery] GetListPaymentRequest request, CancellationToken cancellationToken = default)
    {
        try {
            var userId = User.Claims.FirstOrDefault(x => x.Type == ApplicationClaimConstant.UserId)?.Value;
            var role = User.Claims.FirstOrDefault(x => x.Type == ApplicationClaimConstant.Role)?.Value;

            if (userId is null)
                throw new NotFoundException("User Id not found");

            if (role != UserRoleEnum.User.ToString())
                throw new NotFoundException("Only user can see list payment");

            var mediator = _mediator;
            if (mediator is null)
            {
                var logger = HttpContext.RequestServices.GetRequiredService<ILogger<GetListPaymentController>>();
                logger.LogWarning("Mediator login is null, fallback to GetRequiredService");
                mediator = HttpContext.RequestServices.GetRequiredService<IMediator>();
            }

            var result = await _mediator.Send(new GetListPaymentByUserQuery 
                                                { 
                                                    UserId = Guid.Parse(userId),
                                                    Search = request.Search, 
                                                    OrderColumn = request.OrderColumn, 
                                                    OrderType = request.OrderType, 
                                                    Page = request.Page, 
                                                    Size = request.Size 
                                                });

            return Ok(result);
        } catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}