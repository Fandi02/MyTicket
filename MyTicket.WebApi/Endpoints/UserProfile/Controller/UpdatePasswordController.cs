using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyTicket.Application.Businesses.Auth.Commands;
using MyTicket.Application.Constant;
using MyTicket.Application.Exceptions;
using MyTicket.WebApi.Endpoints.UserProfile.Models.Request;
using Swashbuckle.AspNetCore.Annotations;

namespace MyTicket.WebApi.Endpoints.UserProfile;

public class UpdatePasswordController : BaseEndpointWithoutResponse<UpdatePasswordModelRequest>
{
    private IMediator _mediator;
    public UpdatePasswordController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("update-password")]
    [Authorize]
    [SwaggerOperation(
        Summary = "Update password user",
        Description = "",
        Tags = new[] { "UserProfile" })
    ]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public override async Task<ActionResult> HandleAsync([FromBody] UpdatePasswordModelRequest request, CancellationToken cancellationToken = default)
    {
        try {
            if (request is null)
                throw new BadRequestException("Request is null");

            if (request.PasswordNew != request.ConfirmPassword)
                throw new BadRequestException("New Password and Confirm Password not match");

            var userId = User.Claims.FirstOrDefault(x => x.Type == ApplicationClaimConstant.UserId)?.Value;

            if (userId is null)
                throw new NotFoundException("User Id not found");

            await _mediator.Send(new UpdatePasswordCommand {
                UserId = Guid.Parse(userId),
                PasswordOld = request.PasswordOld,
                PasswordNew = request.PasswordNew
            });

            return Ok();
        } catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}