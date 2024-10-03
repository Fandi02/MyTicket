using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyTicket.WebApi.Endpoints.UserProfile.Models.Response;
using Swashbuckle.AspNetCore.Annotations;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MyTicket.WebApi.Endpoints.Auth;

public class GetuserProfileController : BaseEndpoint<object?, GetUserProfileResponse>
{
    public GetuserProfileController()
    {
    }

    [HttpPost("get-user-profile")]
    [Authorize]
    [SwaggerOperation(
        Summary = "Get detail user profile",
        Description = "",
        Tags = new[] { "UserProfile" })
    ]
    [ProducesResponseType(typeof(GetUserProfileResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    public override async Task<ActionResult<GetUserProfileResponse>> HandleAsync([FromQuery] object request = null, CancellationToken cancellationToken = default)
    {
        try {
            return Ok();
        } catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}