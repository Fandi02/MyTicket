using MediatR;
using Microsoft.AspNetCore.Mvc;
using MyTicket.Application.Exceptions;
using MyTicket.Domain.Entities;
using MyTicket.WebApi.Models.Request.Auth;
using MyTicket.WebApi.Models.Response.Auth;
using MyTicket.WebApi.Services;
using MyTicket.Application.Businesses.Auth.Commands;

namespace MyTicket.WebApi.Controllers;

[ApiController]
[Route(template: "api/v1/auth")]
public class AuthController : BaseApiController
{
    private readonly IConfiguration _configuration;
    public AuthController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [ProducesResponseType(type: typeof(AuthenticationResponse), statusCode: StatusCodes.Status200OK)]
    [HttpPost("login")]
    public async Task<ActionResult<AuthenticationResponse>> Login([FromBody] AuthenticationRequest request)
    {
        if (request is null)
            throw new BadRequestException("Request is null");

        var mediator = Mediator;
        if (mediator is null)
        {
            var logger = HttpContext.RequestServices.GetRequiredService<ILogger<AuthController>>();
            logger.LogWarning("Mediator login is null, fallback to GetRequiredService");
            mediator = HttpContext.RequestServices.GetRequiredService<IMediator>();
        }
        var result = await Mediator.Send(new LoginCommand { Email = request.Email, Password = request.Password, ApplicationCode = request.ApplicationCode });

        var jwtManager = JwtManager;

        if (jwtManager is null)
        {
            var logger = HttpContext.RequestServices.GetRequiredService<ILogger<AuthController>>();
            logger.LogWarning("JwtManager login is null, fallback to GetRequiredService");
            jwtManager = HttpContext.RequestServices.GetRequiredService<ApplicationJwtManager>();
        }

        return TokenBuilder.Build(jwtManager, result);
    }

    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [HttpPost("register")]
    public async Task<ActionResult> Register([FromBody] RegistrationRequest request)
    {
        if (request is null)
            throw new BadRequestException("Request is null");

        if (request.Password != request.ConfirmPassword)
            throw new BadRequestException("Password and confirm password not match");

        if (request.UserRole != UserRoleEnum.Admin && request.UserRole != UserRoleEnum.User)
            throw new BadRequestException("User role not match");

        await Mediator.Send(new RegisterCommand { 
            Email = request.Email, 
            PhoneNumber = request.PhoneNumber,
            FullName = request.FullName,
            UserName = request.UserName,
            Age = request.Age,
            BirthDate = request.BirthDate,
            Password = request.Password, 
            Role = request.UserRole
        });

        return Ok();
    }
}
