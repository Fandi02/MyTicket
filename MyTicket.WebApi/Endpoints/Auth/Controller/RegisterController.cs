using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyTicket.WebApi.Endpoints.Auth.Models.Request;
using Swashbuckle.AspNetCore.Annotations;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MyTicket.WebApi.Endpoints.Auth;

public class RegisterController : BaseEndpointWithoutResponse<RegisterModelRequest>
{
    public RegisterController()
    {
    }

    [HttpPost("register")]
    [AllowAnonymous]
    [SwaggerOperation(
        Summary = "Register",
        Description = "",
        Tags = new[] { "Auth" })
    ]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    public override async Task<ActionResult> HandleAsync([FromBody] RegisterModelRequest request, CancellationToken cancellationToken = default)
    {
        try {
            return Ok();
        } catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}