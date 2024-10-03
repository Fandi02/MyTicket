using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyTicket.WebApi.Endpoints.Auth.Models.Request;
using MyTicket.WebApi.Endpoints.Auth.Models.Response;
using Swashbuckle.AspNetCore.Annotations;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MyTicket.WebApi.Endpoints.Auth;

public class LoginController : BaseEndpoint<LoginModelRequest, LoginModelResponse>
{
    public LoginController()
    {
    }

    [HttpPost("login")]
    [AllowAnonymous]
    [SwaggerOperation(
        Summary = "Login",
        Description = "",
        Tags = new[] { "Auth" })
    ]
    [ProducesResponseType(typeof(LoginModelResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    public override async Task<ActionResult<LoginModelResponse>> HandleAsync([FromBody] LoginModelRequest request, CancellationToken cancellationToken = default)
    {
        try {
            return Ok();
        } catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}