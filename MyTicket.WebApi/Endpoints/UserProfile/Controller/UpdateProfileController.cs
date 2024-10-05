using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyTicket.Application.Businesses.Auth.Commands;
using MyTicket.Application.Constant;
using MyTicket.Application.Exceptions;
using MyTicket.WebApi.Endpoints.UserProfile.Models.Request;
using Swashbuckle.AspNetCore.Annotations;

namespace MyTicket.WebApi.Endpoints.UserProfile;

public class UpdateProfileController : BaseEndpointWithoutResponse<UpdateProfileModelRequest>
{
    private readonly IMediator _mediator;
    public UpdateProfileController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("update-profile")]
    [Authorize]
    [SwaggerOperation(
        Summary = "Update profile user",
        Description = "",
        Tags = new[] { "UserProfile" })
    ]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public override async Task<ActionResult> HandleAsync([FromBody] UpdateProfileModelRequest request, CancellationToken cancellationToken = default)
    {
        try {
            if (request is null)
                throw new BadRequestException("Request is null");

            var userId = User.Claims.FirstOrDefault(x => x.Type == ApplicationClaimConstant.UserId)?.Value;

            if (userId is null)
                throw new NotFoundException("User Id not found");

            await _mediator.Send(new UpdateProfileCommand {
                UserId = Guid.Parse(userId), 
                PhoneNumber = request.PhoneNumber,
                FullName = request.FullName,
                UserName = request.UserName,
                Age = request.Age,
                BirthDate = request.BirthDate
            });

            return Ok();
        } catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}