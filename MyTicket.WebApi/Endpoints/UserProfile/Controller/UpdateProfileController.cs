using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyTicket.WebApi.Endpoints.UserProfile.Models.Request;
using Swashbuckle.AspNetCore.Annotations;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MyTicket.WebApi.Endpoints.UserProfile;

public class UpdateProfileController : BaseEndpointWithoutResponse<UpdateProfileModelRequest>
{
    public UpdateProfileController()
    {
    }

    [HttpPost("update-profile")]
    [Authorize]
    [SwaggerOperation(
        Summary = "Update profile user",
        Description = "",
        Tags = new[] { "UserProfile" })
    ]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    public override async Task<ActionResult> HandleAsync([FromBody] UpdateProfileModelRequest request, CancellationToken cancellationToken = default)
    {
        try {
            return Ok();
        } catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}