using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyTicket.Application.Businesses.Auth.Commands;
using MyTicket.WebApi.Endpoints.Auth.Models.Request;
using MyTicket.WebApi.Endpoints.Auth.Models.Response;
using MyTicket.WebApi.ServiceMessageBroker;
using MyTicket.WebApi.Services;
using Swashbuckle.AspNetCore.Annotations;

namespace MyTicket.WebApi.Endpoints.Auth;

public class LoginController : BaseEndpoint<LoginModelRequest, LoginModelResponse>
{
    private readonly IMessageProducer _messageProducer;
    private ApplicationJwtManager _applicationJwtManager;
    private IMediator _mediator;
    public LoginController(IMessageProducer messageProducer, ApplicationJwtManager applicationJwtManager, IMediator mediator)
    {
        _messageProducer = messageProducer;
        _applicationJwtManager = applicationJwtManager;
        _mediator = mediator;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    [SwaggerOperation(
        Summary = "Login",
        Description = "",
        Tags = new[] { "Auth" })
    ]
    [ProducesResponseType(typeof(LoginModelResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public override async Task<ActionResult<LoginModelResponse>> HandleAsync([FromBody] LoginModelRequest request, CancellationToken cancellationToken = default)
    {
        try {
            var mediator = _mediator;
        
            if (mediator is null)
            {
                var logger = HttpContext.RequestServices.GetRequiredService<ILogger<LoginController>>();
                logger.LogWarning("Mediator login is null, fallback to GetRequiredService");
                mediator = HttpContext.RequestServices.GetRequiredService<IMediator>();
            }

            var result = await _mediator.Send(new LoginCommand { Email = request.Email, Password = request.Password, ApplicationCode = request.ApplicationCode });

            var jwtManager = _applicationJwtManager;

            if (jwtManager is null)
            {
                var logger = HttpContext.RequestServices.GetRequiredService<ILogger<LoginController>>();
                logger.LogWarning("JwtManager login is null, fallback to GetRequiredService");
                jwtManager = HttpContext.RequestServices.GetRequiredService<ApplicationJwtManager>();
            }

            return TokenBuilder.Build(jwtManager, result);
        } catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}