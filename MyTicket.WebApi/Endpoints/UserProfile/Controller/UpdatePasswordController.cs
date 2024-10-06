using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyTicket.Application.Businesses.Auth.Commands;
using MyTicket.Application.Constant;
using MyTicket.Application.Exceptions;
using MyTicket.WebApi.Endpoints.UserProfile.Models.Request;
using MyTicket.WebApi.ServiceMessageBroker;
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
            var email = User.Claims.FirstOrDefault(x => x.Type == ApplicationClaimConstant.Email)?.Value;
            var fullName = User.Claims.FirstOrDefault(x => x.Type == ApplicationClaimConstant.FullName)?.Value;

            if (userId is null)
                throw new NotFoundException("User Id not found");

            await _mediator.Send(new UpdatePasswordCommand {
                UserId = Guid.Parse(userId),
                PasswordOld = request.PasswordOld,
                PasswordNew = request.PasswordNew
            });

            var sendEmail = new 
            {
                Email = email,
                FullName = fullName,
            };

            var producer = new MessageProducer();
            producer.SendingMessage("email_queue_update_password", sendEmail);

            return Ok();
        } catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}