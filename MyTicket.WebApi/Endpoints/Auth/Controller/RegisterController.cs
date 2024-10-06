using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using MyTicket.Application.Exceptions;
using MyTicket.Domain.Entities;
using MyTicket.WebApi.Endpoints.Auth.Models.Request;
using MyTicket.WebApi.ServiceMessageBroker;
using Swashbuckle.AspNetCore.Annotations;

namespace MyTicket.WebApi.Endpoints.Auth;

public class RegisterController : BaseEndpointWithoutResponse<RegisterModelRequest>
{
    private readonly IMediator _mediator;
    private readonly IEmailSender _emailSender;
    private readonly IConfiguration _configuration;
    public RegisterController(IMediator mediator, IEmailSender emailSender, IConfiguration configuration)
    {
        _mediator = mediator;
        _emailSender = emailSender;
        _configuration = configuration;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    [SwaggerOperation(
        Summary = "Register",
        Description = "",
        Tags = new[] { "Auth" })
    ]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public override async Task<ActionResult> HandleAsync([FromBody] RegisterModelRequest request, CancellationToken cancellationToken = default)
    {
        try {
            if (request is null)
                throw new BadRequestException("Request is null");

            if (request.Password != request.ConfirmPassword)
                throw new BadRequestException("Password and confirm password not match");

            if (request.UserRole != UserRoleEnum.Admin && request.UserRole != UserRoleEnum.User)
                throw new BadRequestException("User role not match");

            // await _mediator.Send(new RegisterCommand { 
            //     Email = request.Email, 
            //     PhoneNumber = request.PhoneNumber,
            //     FullName = request.FullName,
            //     UserName = request.UserName,
            //     Age = request.Age,
            //     BirthDate = request.BirthDate,
            //     Password = request.Password, 
            //     Role = request.UserRole
            // });

            var producer = new MessageProducer();
            producer.SendingMessage("email_queue", request);

            return Ok();
        } catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}