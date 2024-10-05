using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyTicket.Application.Constant;
using MyTicket.Application.Exceptions;
using MyTicket.Application.UserProfile.Queries;
using MyTicket.WebApi.Endpoints.UserProfile.Models.Response;
using Swashbuckle.AspNetCore.Annotations;

namespace MyTicket.WebApi.Endpoints.UserProfile;

public class GetuserProfileController : BaseEndpoint<object?, GetUserProfileResponse>
{
    private readonly IMediator _mediator;
    public GetuserProfileController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("get-user-profile")]
    [Authorize]
    [SwaggerOperation(
        Summary = "Get detail user profile",
        Description = "",
        Tags = new[] { "UserProfile" })
    ]
    [ProducesResponseType(typeof(GetUserProfileResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public override async Task<ActionResult<GetUserProfileResponse>> HandleAsync([FromQuery] object request = null, CancellationToken cancellationToken = default)
    {
        try {
            var userId = User.Claims.FirstOrDefault(x => x.Type == ApplicationClaimConstant.UserId)?.Value;
            var role = User.Claims.FirstOrDefault(x => x.Type == ApplicationClaimConstant.Role)?.Value;

            if (userId is null)
                throw new NotFoundException("User Id not found");

            var mediator = _mediator;
            if (mediator is null)
            {
                var logger = HttpContext.RequestServices.GetRequiredService<ILogger<GetuserProfileController>>();
                logger.LogWarning("Mediator login is null, fallback to GetRequiredService");
                mediator = HttpContext.RequestServices.GetRequiredService<IMediator>();
            }

            var result = await _mediator.Send(new GetProfileQuery { UserId = Guid.Parse(userId), UserRole = role });

            return Ok(result);
        } catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}