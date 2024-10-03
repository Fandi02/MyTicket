using MediatR;
using Microsoft.AspNetCore.Mvc;
using MyTicket.Application.Exceptions;
using MyTicket.Application.Businesses.Auth.Commands;
using MyTicket.Application.Services;
using MyTicket.Application.UserProfile.Queries;
using MyTicket.WebApi.Models.Response.UserProfile;
using MyTicket.WebApi.Models.Request.UserProfile;
using Microsoft.AspNetCore.Authorization;
using MyTicket.Application.Interfaces;
using MyTicket.Application.Constant;

namespace MyTicket.WebApi.Controllers;

[Authorize]
[ApiController]
[Route(template: "api/v1/user-profile")]
public class UserProfileController : BaseApiController
{
    private readonly IConfiguration _configuration;
    private readonly IContext _context;
    public UserProfileController(IConfiguration configuration, IContext context)
    {
        _configuration = configuration;
        _context = context;
    }

    [ProducesResponseType(type: typeof(UserProfileResponse), statusCode: StatusCodes.Status200OK)]
    [HttpGet("detail")]
    public async Task<ActionResult<UserProfileResponse>> DetailUser()
    {
        var userId = User.Claims.FirstOrDefault(x => x.Type == ApplicationClaimConstant.UserId)?.Value;
        var role = User.Claims.FirstOrDefault(x => x.Type == ApplicationClaimConstant.Role)?.Value;

        if (userId is null)
            throw new NotFoundException("User Id not found");

        var mediator = Mediator;
        if (mediator is null)
        {
            var logger = HttpContext.RequestServices.GetRequiredService<ILogger<UserProfileController>>();
            logger.LogWarning("Mediator login is null, fallback to GetRequiredService");
            mediator = HttpContext.RequestServices.GetRequiredService<IMediator>();
        }

        var result = await Mediator.Send(new GetProfileQuery { UserId = Guid.Parse(userId), UserRole = role });

        return Ok(result);
    }

    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [HttpPost("update-profile")]
    public async Task<ActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
    {
        if (request is null)
            throw new BadRequestException("Request is null");

        var userId = User.Claims.FirstOrDefault(x => x.Type == ApplicationClaimConstant.UserId)?.Value;

        if (userId is null)
            throw new NotFoundException("User Id not found");

        await Mediator.Send(new UpdateProfileCommand {
            UserId = Guid.Parse(userId), 
            PhoneNumber = request.PhoneNumber,
            FullName = request.FullName,
            UserName = request.UserName,
            Age = request.Age,
            BirthDate = request.BirthDate
        });

        return Ok();
    }

    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [HttpPost("update-password")]
    public async Task<ActionResult> UpdatePassword([FromBody] UpdatePasswordRequest request)
    {
        if (request is null)
            throw new BadRequestException("Request is null");

        if (request.PasswordNew != request.ConfirmPassword)
            throw new BadRequestException("New Password and Confirm Password not match");

        var userId = User.Claims.FirstOrDefault(x => x.Type == ApplicationClaimConstant.UserId)?.Value;

        if (userId is null)
            throw new NotFoundException("User Id not found");

        await Mediator.Send(new UpdatePasswordCommand {
            UserId = Guid.Parse(userId),
            PasswordOld = request.PasswordOld,
            PasswordNew = request.PasswordNew
        });

        return Ok();
    }
}
