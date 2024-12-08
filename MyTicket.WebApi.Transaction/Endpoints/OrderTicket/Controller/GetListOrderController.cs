using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyTicket.Application.Businesses.OrderTicket.Queries;
using MyTicket.Application.Constant;
using MyTicket.Application.Exceptions;
using MyTicket.WebApi.Transaction.Endpoints;
using MyTicket.WebApi.Transaction.Endpoints.OrderTicket.Models.Request;
using MyTicket.WebApi.Transaction.Endpoints.OrderTicket.Models.Response;
using Swashbuckle.AspNetCore.Annotations;

namespace MyTicket.WebApi.Endpoints.OrderTicket;

public class GetListOrderController : BaseEndpoint<GetListOrderRequest, GetListOrderResponse>
{
    private readonly IMediator _mediator;
    public GetListOrderController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [Authorize]
    [SwaggerOperation(
        Summary = "Get list order ticket",
        Description = "",
        Tags = new[] { "OrderTicket" })
    ]
    [ProducesResponseType(typeof(GetListOrderResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public override async Task<ActionResult<GetListOrderResponse>> HandleAsync([FromQuery] GetListOrderRequest request, CancellationToken cancellationToken = default)
    {
        var userId = User.Claims.FirstOrDefault(x => x.Type == ApplicationClaimConstant.UserId)?.Value;
        var role = User.Claims.FirstOrDefault(x => x.Type == ApplicationClaimConstant.Role)?.Value;

        if (userId is null)
            throw new NotFoundException("User Id not found");

        var mediator = _mediator;
        if (mediator is null)
        {
            var logger = HttpContext.RequestServices.GetRequiredService<ILogger<GetListOrderController>>();
            logger.LogWarning("Mediator login is null, fallback to GetRequiredService");
            mediator = HttpContext.RequestServices.GetRequiredService<IMediator>();
        }

        var result = await _mediator.Send(new GetListOrderQuery 
                    { 
                        Search = request.Search, 
                        OrderColumn = request.OrderColumn, 
                        OrderType = request.OrderType, 
                        Page = request.Page, 
                        Size = request.Size,
                        Role = role,
                        UserId = Guid.Parse(userId)
                    });

        return Ok(result);
    }
}